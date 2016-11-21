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

        public static Instruction LoadType(Integer id, IMetadataDatabase metadataDatabase)
        {
            var value = metadataDatabase.LoadType(id);
            return new Instruction(Opcode.LoadType, value);
        }

        public static new Instruction GetType()
        {
            return new Instruction(Opcode.GetType);
        }

        public static Instruction TypeName()
        {
            return new Instruction(Opcode.TypeName);
        }

        public static Instruction TypeBaseType()
        {
            return new Instruction(Opcode.TypeBaseType);
        }

        public static Instruction TypeIs()
        {
            return new Instruction(Opcode.TypeIs);
        }

        public static Instruction TypeEquals()
        {
            return new Instruction(Opcode.TypeEquals);
        }

        public static Instruction Return()
        {
            return new Instruction(Opcode.Return);
        }
    }
}
