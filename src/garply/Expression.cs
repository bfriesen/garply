using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                    case Opcode.LoadOpcode:
                        {
                            var opcode = (Opcode)(ushort)instruction.Operand.Raw;
                            context.Push(new Value(opcode));
                            break;
                        }
                    case Opcode.LoadString:
                        {
                            Debug.Assert(instruction.Operand.Type == Types.@string);
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
                            Debug.Assert(rhsValue.Type == Types.type);
                            var rhsType = (Types)(uint)rhsValue.Raw;

                            var lhsValue = context.Pop();
                            Debug.Assert(lhsValue.Type == Types.type);
                            var lhsType = (Types)(uint)lhsValue.Raw;

                            var lhsTypeIsRhsType = (lhsType & rhsType) != 0 && lhsType >= rhsType;
                            context.Push(new Value(lhsTypeIsRhsType));
                            break;
                        }
                    case Opcode.TypeEquals:
                        {
                            var rhsValue = context.Pop();
                            Debug.Assert(rhsValue.Type == Types.type);
                            var rhsType = (Types)(uint)rhsValue.Raw;

                            var lhsValue = context.Pop();
                            Debug.Assert(lhsValue.Type == Types.type);
                            var lhsType = (Types)(uint)lhsValue.Raw;

                            var lhsTypeEqualsRhsType = lhsType == rhsType;
                            context.Push(new Value(lhsTypeEqualsRhsType));
                            break;
                        }
                    case Opcode.TupleArity:
                        {
                            var tupleValue = context.Pop();
                            Debug.Assert(tupleValue.Type == Types.tuple);
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
                    case Opcode.NewExpression:
                        {
                            var instructionCount = (int)instruction.Operand.Raw;
                            var typeValue = context.Pop();
                            Debug.Assert(typeValue.Type == Types.type);
                            var type = (Types)(uint)typeValue.Raw;
                            var instructions = new Instruction[instructionCount];
                            for (int j = 0; j < instructionCount; j++)
                            {
                                var instructionTupleValue = context.Pop();
                                var instructionTuple = Heap.GetTuple((int)instructionTupleValue.Raw);
                                Debug.Assert(instructionTuple.Items.Count == 2);
                                Debug.Assert(instructionTuple.Items[0].Type == Types.opcode);
                                instructions[j] = Instruction.FromTuple(instructionTuple);
                                instructionTupleValue.RemoveRef();
                            }
                            var expressionValue = Heap.AllocateExpression(type, instructions.ToArray());
                            context.Push(expressionValue);
                            expressionValue.AddRef();
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
                            var tail = new Value(Types.list, list.TailIndex);
                            context.Push(tail);
                            tail.AddRef();
                            listValue.RemoveRef();
                            break;
                        }
                    case Opcode.AssignVariable:
                        {
                            var value = context.Pop();
                            var variableIndex = (int)instruction.Operand.Raw;
                            var result = context.Scope.SetValue(context, variableIndex, value, false);
                            if (result.Type == Types.error) return default(Value);
                            context.Push(result);
                            break;
                        }
                    case Opcode.AssignMutableVariable:
                        {
                            var value = context.Pop();
                            var variableIndex = (int)instruction.Operand.Raw;
                            var result = context.Scope.SetValue(context, variableIndex, value, true);
                            if (result.Type == Types.error) return default(Value);
                            context.Push(result);
                            break;
                        }
                    case Opcode.ReadVariable:
                        {
                            Debug.Assert(instruction.Operand.Type == Types.@int);
                            var value = context.Scope.GetValue((int)instruction.Operand.Raw);
                            if (value.Type == Types.error) return default(Value);
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

        public string ToString(bool shortForm) => shortForm
            ? $"expr<{Type}>[{_instructions.Length}]"
            : ToString();

        public override string ToString() => $@"expr<{Type}>[
  {string.Join(@",
  ", _instructions)}
]";
    }
}
