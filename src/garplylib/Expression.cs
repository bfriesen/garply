using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace garply
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

        public IFirstClassType Evaluate(IExecutionContext context)
        {
            var arg = context.Pop();

            for (int i = 0; i < _instructions.Length; i++)
            {
                var instruction = _instructions[i];

                switch (instruction.Opcode)
                {
                    case Opcode.LoadInteger:
                    case Opcode.LoadFloat:
                    case Opcode.LoadBoolean:
                    case Opcode.LoadString:
                    case Opcode.LoadType:
                        {
                            context.Push((IFirstClassType)instruction.Operand);
                            break;
                        }
                    case Opcode.GetType:
                        {
                            var value = context.Pop();
                            var type = value.Type;
                            context.Push(type);
                            break;
                        }
                    case Opcode.TypeName:
                        {
                            var type = (IType)context.Pop();
                            context.Push(type.Name);
                            break;
                        }
                    case Opcode.TypeBaseType:
                        {
                            var type = (IType)context.Pop();
                            context.Push(type.BaseType);
                            break;
                        }
                    case Opcode.TypeIs:
                        {
                            var otherType = (IType)context.Pop();
                            var type = (IType)context.Pop();
                            context.Push(type.Is(otherType));
                            break;
                        }
                    case Opcode.TupleArity:
                        {
                            var tuple = (Tuple)context.Pop();
                            var arity = tuple.Arity;
                            context.Push(arity);
                            break;
                        }
                    case Opcode.TupleItem:
                        {
                            var tuple = (Tuple)context.Pop();
                            var index = (Integer)instruction.Operand;
                            var item = tuple.GetItem(index);
                            context.Push(item);
                            break;
                        }
                    case Opcode.NewTuple:
                        {
                            var arity = (Integer)instruction.Operand;
                            var tuple = new Tuple(arity, context);
                            context.Push(tuple);
                            break;
                        }
                    case Opcode.ListEmpty:
                        {
                            var list = List.Empty;
                            context.Push(list);
                            break;
                        }
                    case Opcode.ListAdd:
                        {
                            var item = context.Pop();
                            var list = (List)context.Pop();
                            context.Push(list.Add(item));
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

#if UNSTABLE
            throw new InvalidOperationException("No Return instruction was encountered");
#else
            // TODO: Push an error onto the error stack
            //context.Push(new Error("No return"))
            return ErrorValue.Instance;
#endif
        }
    }
}
