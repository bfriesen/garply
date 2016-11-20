namespace garply
{
    public static class Instructions
    {
        private static readonly byte[] _empty = new byte[0];

        public static Instruction Nop()
        {
            return new Instruction(Opcode.Nop, default(EmptyOperand));
        }
    }
}
