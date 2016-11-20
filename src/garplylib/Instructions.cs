using System;

namespace garply
{
    public static class Instructions
    {
        private static readonly byte[] _empty = new byte[0];

        public static Instruction Nop()
        {
            return new Instruction(Opcode.Nop, default(EmptyOperand));
        }

        public static Instruction LoadInteger(Integer value)
        {
            return new Instruction(Opcode.LoadInteger, value);
        }

        public static Instruction LoadFloat(Float value)
        {
            return new Instruction(Opcode.LoadFloat, value);
        }

        public static Instruction LoadBoolean(Boolean value)
        {
            return new Instruction(Opcode.LoadBoolean, value);
        }

        public static Instruction LoadString(Integer id, IMetadataDatabase metadataDatabase)
        {
            var value = metadataDatabase.LoadString(id);
            return new Instruction(Opcode.LoadString, value);
        }

        public static Instruction Return()
        {
            return new Instruction(Opcode.Return);
        }
    }
}
