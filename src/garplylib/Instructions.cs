namespace Garply
{
    public static class Instructions
    {
        private static readonly byte[] _empty = new byte[0];

        public static Instruction Nop()
        {
            return new Instruction(Opcode.Nop);
        }

        public static Instruction LoadInteger(long value)
        {
            return new Instruction(Opcode.LoadInteger, new Value(value));
        }

        public static Instruction LoadFloat(double value)
        {
            return new Instruction(Opcode.LoadFloat, new Value(value));
        }

        public static Instruction LoadBoolean(bool value)
        {
            return new Instruction(Opcode.LoadBoolean, new Value(value));
        }

        public static Instruction LoadString(long id, IMetadataDatabase metadataDatabase)
        {
            var value = metadataDatabase.LoadString(id);
            return new Instruction(Opcode.LoadString, value);
        }

        public static Instruction LoadType(Types type)
        {
            return new Instruction(Opcode.LoadType, new Value(type));
        }

        public static new Instruction GetType()
        {
            return new Instruction(Opcode.GetType);
        }

        public static Instruction TypeIs()
        {
            return new Instruction(Opcode.TypeIs);
        }

        public static Instruction TypeEquals()
        {
            return new Instruction(Opcode.TypeEquals);
        }

        public static Instruction TupleArity()
        {
            return new Instruction(Opcode.TupleArity);
        }

        public static Instruction TupleItem(int index)
        {
            return new Instruction(Opcode.TupleItem, new Value(index));
        }

        public static Instruction NewTuple(int arity)
        {
            return new Instruction(Opcode.NewTuple, new Value(arity));
        }

        public static Instruction ListEmpty()
        {
            return new Instruction(Opcode.ListEmpty);
        }

        public static Instruction ListAdd()
        {
            return new Instruction(Opcode.ListAdd);
        }

        public static Instruction ListHead()
        {
            return new Instruction(Opcode.ListHead);
        }

        public static Instruction ListTail()
        {
            return new Instruction(Opcode.ListTail);
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
