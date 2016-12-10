using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Garply
{
    internal struct Expression
    {
        private static readonly Instruction[] _emptyInstructions = new Instruction[0];

        private Instruction[] _instructions;

        public Expression(Types type, Instruction[] instructions)
        {
            Type = type;
            _instructions = instructions;
        }

        public Types Type { get; }

        public IReadOnlyList<Instruction> Instructions
        {
            get { return _instructions ?? (_instructions = _emptyInstructions); }
        }

        public bool IsEmpty => _instructions == null || _instructions.Length == 0;

        public static Expression Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var expression = new ExpressionBuilder();

                var expressionType = (Types)reader.ReadUInt32();
                expression.SetType(expressionType);

                var instructionCount = reader.ReadInt32();
                for (int i = 0; i < instructionCount; i++)
                {
                    expression.Add(Instruction.Read(stream));
                }

                return expression.Build();
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((uint)Type);
            writer.Write(_instructions.Length);

            for (int i = 0; i < _instructions.Length; i++)
            {
                _instructions[i].Write(writer);
            }
        }

        public Value Evaluate(ExecutionContext context)
        {
            if (IsEmpty) return default(Value);

            var originalSize = context.Size;

            for (int i = 0; i < _instructions.Length; i++)
            {
                var instruction = _instructions[i];

                switch (instruction.Opcode)
                {
                    case Opcode.LoadInteger:
                    case Opcode.LoadFloat:
                    case Opcode.LoadBoolean:
                        context.Push(instruction.Operand);
                        break;
                    case Opcode.LoadType:
                        {
                            var type = (Types)(uint)instruction.Operand.Raw;
                            context.Push(new Value(type));
                            break;
                        }
                    case Opcode.LoadString:
                        {
                            Debug.Assert(instruction.Operand.Type == Types.String);
                            var rawValue = StringDatabase.GetRawValue(instruction.Operand.Raw);
                            var value = Heap.AllocatePersistentString(rawValue);
                            context.Push(value);
                            value.AddRef();
                            break;
                        }
                    case Opcode.GetType:
                        {
                            var value = context.Pop();
                            var type = value.Type;
                            context.Push(new Value(type));
                            value.RemoveRef();
                            break;
                        }
                    case Opcode.TypeIs:
                        {
                            var rhsValue = context.Pop();
                            Debug.Assert(rhsValue.Type == Types.Type);
                            var rhsType = (Types)(uint)rhsValue.Raw;

                            var lhsValue = context.Pop();
                            Debug.Assert(lhsValue.Type == Types.Type);
                            var lhsType = (Types)(uint)lhsValue.Raw;

                            var lhsTypeIsRhsType = (lhsType & rhsType) != 0 && lhsType >= rhsType;
                            context.Push(new Value(lhsTypeIsRhsType));
                            break;
                        }
                    case Opcode.TypeEquals:
                        {
                            var rhsValue = context.Pop();
                            Debug.Assert(rhsValue.Type == Types.Type);
                            var rhsType = (Types)(uint)rhsValue.Raw;

                            var lhsValue = context.Pop();
                            Debug.Assert(lhsValue.Type == Types.Type);
                            var lhsType = (Types)(uint)lhsValue.Raw;

                            var lhsTypeEqualsRhsType = lhsType == rhsType;
                            context.Push(new Value(lhsTypeEqualsRhsType));
                            break;
                        }
                    case Opcode.TupleArity:
                        {
                            var tupleValue = context.Pop();
                            Debug.Assert(tupleValue.Type == Types.Tuple);
                            var tuple = Heap.GetTuple((int)tupleValue.Raw);
                            var arity = tuple.Items.Count;
                            context.Push(new Value(arity));
                            tupleValue.RemoveRef();
                            break;
                        }
                    case Opcode.TupleItem:
                        {
                            var tupleValue = context.Pop();
                            var tuple = Heap.GetTuple((int)tupleValue.Raw);
                            var index = (int)instruction.Operand.Raw;
                            var item = tuple.Items[index];
                            context.Push(item);
                            item.AddRef();
                            tupleValue.RemoveRef();
                            break;
                        }
                    case Opcode.NewTuple:
                        {
                            var arity = (int)instruction.Operand.Raw;
                            var tuple = Heap.AllocateTuple(arity, context);
                            context.Push(tuple);
                            tuple.AddRef();
                            break;
                        }
                    case Opcode.ListEmpty:
                        {
                            context.Push(Empty.List);
                            break;
                        }
                    case Opcode.ListAdd:
                        {
                            var head = context.Pop();
                            var tail = context.Pop();
                            // No need to remove ref for head and tail - their refs are "taken" by the new list.
                            var list = Heap.AllocateList(head, tail);
                            context.Push(list);
                            list.AddRef();
                            break;
                        }
                    case Opcode.ListHead:
                        {
                            var listValue = context.Pop();
                            var list = Heap.GetList((int)listValue.Raw);
                            var head = list.Head;
                            context.Push(head);
                            listValue.RemoveRef();
                            break;
                        }
                    case Opcode.ListTail:
                        {
                            var listValue = context.Pop();
                            var list = Heap.GetList((int)listValue.Raw);
                            var tail = new Value(Types.List, list.TailIndex);
                            context.Push(tail);
                            tail.AddRef();
                            listValue.RemoveRef();
                            break;
                        }
                    case Opcode.AssignVariable:
                        {
                            var value = context.Pop();
                            var result = context.Scope.SetValue(context, instruction.Operand, value, false);
                            if (result.Type == Types.Error) return default(Value);
                            context.Push(result);
                            break;
                        }
                    case Opcode.AssignMutableVariable:
                        {
                            var value = context.Pop();
                            var result = context.Scope.SetValue(context, instruction.Operand, value, true);
                            if (result.Type == Types.Error) return default(Value);
                            context.Push(result);
                            break;
                        }
                    case Opcode.ReadVariable:
                        {
                            Debug.Assert(instruction.Operand.Type == Types.Integer);
                            var value = context.Scope.GetValue((int)instruction.Operand.Raw);
                            if (value.Type == Types.Error) return default(Value);
                            context.Push(value);
                            value.AddRef();
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException("opcode");
                }
            }

            if (context.Size == 0)
            {
                context.AddError(new Error($"Invalid expression - evaluation stack was empty upon exit."));
                return default(Value);
            }

            var returnValue = context.Pop();

            if (context.Size != originalSize)
            {
                context.AddError(new Error($"Invalid expression - evaluation stack size was modified. Original size: {originalSize}, size upon exist: {context.Size}."));
                return default(Value);
            }

            return returnValue;
        }

        public override string ToString() => $"Expression<{Type}>";
    }
}
