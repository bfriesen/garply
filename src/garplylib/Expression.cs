using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Garply
{
    public struct Expression
    {
        private static readonly Instruction[] _emptyInstructions = new Instruction[0];

        private Instruction[] _instructions;

        public Expression(Instruction[] instructions)
        {
            _instructions = instructions;
        }

        public IReadOnlyList<Instruction> Instructions
        {
            get { return _instructions ?? (_instructions = _emptyInstructions); }
        }

        public static Expression Read(Stream stream, IMetadataDatabase metadataDatabase)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var expression = new ExpressionBuilder();

                var instructionCount = reader.ReadInt32();

                for (int i = 0; i < instructionCount; i++)
                {
                    expression.Add(Instruction.Read(stream, metadataDatabase));
                }

                return expression.Build();
            }
        }

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            writer.Write(_instructions.Length);

            for (int i = 0; i < _instructions.Length; i++)
            {
                _instructions[i].Write(writer, metadataDatabase);
            }
        }

        public Value Evaluate(IExecutionContext context)
        {
            var arg = context.Pop();
            Heap.IncrementRefCount(arg);

            for (int i = 0; i < _instructions.Length; i++)
            {
                var instruction = _instructions[i];

                switch (instruction.Opcode)
                {
                    case Opcode.LoadInteger:
                    case Opcode.LoadFloat:
                    case Opcode.LoadBoolean:
                    case Opcode.LoadString:
                        context.Push(instruction.Operand);
                        break;
                    case Opcode.LoadType:
                        {
                            var type = (Types)(uint)instruction.Operand.Raw;
                            context.Push(new Value(type));
                            break;
                        }
                    case Opcode.GetType:
                        {
                            var type = context.Pop().Type;
                            context.Push(new Value(type));
                            break;
                        }
                    case Opcode.TypeIs:
                        {
                            var value = context.Pop();
                            Debug.Assert(value.Type == Types.Type);
                            var rhsType = (Types)(uint)value.Raw;

                            value = context.Pop();
                            Debug.Assert(value.Type == Types.Type);
                            var lhsType = (Types)(uint)value.Raw;

                            var lhsTypeIsRhsType = (lhsType & rhsType) != 0 && lhsType >= rhsType;
                            context.Push(new Value(lhsTypeIsRhsType));
                            break;
                        }
                    case Opcode.TypeEquals:
                        {
                            var value = context.Pop();
                            Debug.Assert(value.Type == Types.Type);
                            var rhsType = (Types)(uint)value.Raw;

                            value = context.Pop();
                            Debug.Assert(value.Type == Types.Type);
                            var lhsType = (Types)(uint)value.Raw;

                            var lhsTypeEqualsRhsType = lhsType == rhsType;
                            context.Push(new Value(lhsTypeEqualsRhsType));
                            break;
                        }
                    case Opcode.TupleArity:
                        {
                            var tuple = Heap.GetTuple((int)context.Pop().Raw);
                            var arity = tuple.Items.Count;
                            context.Push(new Value(arity));
                            break;
                        }
                    case Opcode.TupleItem:
                        {
                            var tupleValue = context.Pop();
                            var tuple = Heap.GetTuple((int)tupleValue.Raw);
                            var index = (int)instruction.Operand.Raw;
                            var item = tuple.Items[index];
                            context.Push(item);
                            break;
                        }
                    case Opcode.NewTuple:
                        {
                            var arity = (int)instruction.Operand.Raw;
                            var tuple = Heap.AllocateTuple(arity, context);
                            context.Push(tuple);
                            break;
                        }
                    case Opcode.ListEmpty:
                        {
                            var list = Empty.List;
                            context.Push(list);
                            break;
                        }
                    case Opcode.ListAdd:
                        {
                            var head = context.Pop();
                            var tail = context.Pop();
                            var list = Heap.AllocateList(head, tail);
                            context.Push(list);
                            break;
                        }
                    case Opcode.ListHead:
                        {
                            var listValue = context.Pop();
                            var list = Heap.GetList((int)listValue.Raw);
                            var head = list.Head;
                            context.Push(head);
                            break;
                        }
                    case Opcode.ListTail:
                        {
                            var listValue = context.Pop();
                            var list = Heap.GetList((int)listValue.Raw);
                            var tail = new Value(Types.List, list.TailIndex);
                            context.Push(tail);
                            break;
                        }
                    case Opcode.PushArg:
                        {
                            context.Push(arg);
                            break;
                        }
                    case Opcode.Return:
                        {
                            var returnValue = context.Pop();
                            Heap.IncrementRefCount(returnValue);
                            Heap.DecrementRefCount(arg);
                            return returnValue;
                        }
                }
            }

            throw new InvalidOperationException("FATAL: No return instruction was encountered");
        }
    }
}
