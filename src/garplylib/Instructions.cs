namespace Garply
{
    public static class Instructions
    {
        private static readonly byte[] _empty = new byte[0];

        public static Instruction Nop()
        {
            return new Instruction(Opcode.Nop);
        }

        public static Instruction LoadInteger(Integer value)
        {
            return new Instruction(Opcode.LoadInteger, new Value(value, true));
        }

        public static Instruction LoadFloat(Float value)
        {
            return new Instruction(Opcode.LoadFloat, new Value(value, true));
        }

        public static Instruction LoadBoolean(Boolean value)
        {
            return new Instruction(Opcode.LoadBoolean, new Value(value, true));
        }

        public static Instruction LoadString(Integer id, IMetadataDatabase metadataDatabase)
        {
            var value = metadataDatabase.LoadString(id);
            return new Instruction(Opcode.LoadString, new Value(value, true));
        }

        public static Instruction LoadType(Types type)
        {
            return new Instruction(Opcode.LoadType, new Value(TypeValue.Get(type), true));
        }

        public static new Instruction GetType()
        {
            return new Instruction(Opcode.GetType);
        }

        public static Instruction TypeIs()
        {
            return new Instruction(Opcode.TypeIs);
        }

        //public static Instruction TypeEquals()
        //{
        //    return new Instruction(Opcode.TypeEquals);
        //}

        public static Instruction TupleArity()
        {
            return new Instruction(Opcode.TupleArity);
        }

        public static Instruction TupleItem(Integer index)
        {
            return new Instruction(Opcode.TupleItem, new Value(index, true));
        }

        public static Instruction NewTuple(Integer arity)
        {
            return new Instruction(Opcode.NewTuple, new Value(arity, true));
        }

        public static Instruction ListEmpty()
        {
            return new Instruction(Opcode.ListEmpty);
        }

        public static Instruction ListAdd()
        {
            return new Instruction(Opcode.ListAdd);
        }

        public static Instruction PushArg()
        {
            return new Instruction(Opcode.PushArg);
        }

        public static Instruction Return()
        {
            return new Instruction(Opcode.Return);
        }
    }
}
