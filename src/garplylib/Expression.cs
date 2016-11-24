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

            for (int i = 0; i < _instructions.Length; i++)
            {
                var instruction = _instructions[i];

                switch (instruction.Opcode)
                {
                    case Opcode.LoadInteger:
                        context.Push(new Value(instruction.Operand.AsInteger));
                        break;
                    case Opcode.LoadFloat:
                        context.Push(new Value(instruction.Operand.AsFloat));
                        break;
                    case Opcode.LoadBoolean:
                        context.Push(new Value(instruction.Operand.AsBoolean));
                        break;
                    case Opcode.LoadString:
                        context.Push(new Value(instruction.Operand.AsString));
                        break;
                    case Opcode.LoadType:
                        context.Push(new Value(instruction.Operand.AsType));
                        break;
                    case Opcode.GetType:
                        {
                            var type = context.Pop().Type;
                            context.Push(new Value(TypeValue.Get(type)));
                            break;
                        }
                    case Opcode.TypeIs:
                        {
                            Value value = context.Pop();
                            Debug.Assert(value.Type == Types.Type);
                            var otherType = value.AsType.Type;

                            value = context.Pop();
                            Debug.Assert(value.Type == Types.Type);
                            var type = value.AsType.Type;
                            var typeIsOtherType = (type & otherType) == otherType;
                            context.Push(new Value(Boolean.Get(typeIsOtherType)));
                            break;
                        }
                    case Opcode.TupleArity:
                        {
                            var tuple = context.Pop().AsTuple;
                            var arity = tuple.Arity;
                            context.Push(new Value(arity));
                            break;
                        }
                    case Opcode.TupleItem:
                        {
                            var tuple = context.Pop().AsTuple;
                            var index = instruction.Operand.AsInteger;
                            var item = tuple.GetItem(index);
                            context.Push(item);
                            break;
                        }
                    case Opcode.NewTuple:
                        {
                            var arity = instruction.Operand.AsInteger;
                            var tuple = new Tuple(arity, context);
                            context.Push(new Value(tuple));
                            break;
                        }
                    case Opcode.ListEmpty:
                        {
                            var list = List.Empty;
                            context.Push(new Value(list));
                            break;
                        }
                    case Opcode.ListAdd:
                        {
                            var item = context.Pop();
                            var list = context.Pop().AsList;
                            context.Push(new Value(list.Add(item)));
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
                            return returnValue;
                        }
                }
            }

            throw new InvalidOperationException("FATAL: No return instruction was encountered");
        }
    }
}
