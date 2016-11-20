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

        public static Instruction Load(Integer value)
        {
            return new Instruction(Opcode.LoadInteger, value);
        }

        public static Instruction Load(Float value)
        {
            return new Instruction(Opcode.LoadFloat, value);
        }

        public static Instruction Load(Boolean value)
        {
            return new Instruction(Opcode.LoadBoolean, value);
        }
    }
}
