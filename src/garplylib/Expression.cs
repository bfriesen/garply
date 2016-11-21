using System.Collections.Generic;
using System.IO;
using System.Text;

namespace garply
{
    public struct Expression
    {
        private static readonly Instruction[] _emptyInstructions = new Instruction[0];

        private Instruction[] _instructions;

        public Expression(Instruction[] instructions, byte arity, byte constants, byte variables)
        {
            _instructions = instructions;
            Arity = arity;
            Constants = constants;
            Variables = variables;
        }

        public IReadOnlyList<Instruction> Instructions
        {
            get { return _instructions ?? (_instructions = _emptyInstructions); }
        }

        public byte Arity { get; }
        public byte Constants { get; }
        public byte Variables { get; }

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

            foreach (var instruction in _instructions)
            {
                instruction.Write(writer, metadataDatabase);
            }
        }
    }
}
