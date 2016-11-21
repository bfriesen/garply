namespace garply
{
    public enum Opcode : ushort
    {
        Nop,
        GetType,
        TypeName,
        TypeBaseType,
        TypeIs,
        //TypeEquals,   // TODO
        TupleArity,
        TupleItem,
        //ListHead,     // TODO
        //ListTail,     // TODO
        NewTuple,
        ListEmpty,
        ListAdd,
        PushArg,
        LoadBoolean,
        LoadString,
        LoadInteger,
        LoadFloat,
        LoadType,
        //Return,
        Reserved1 = Instruction.MarkerByte1,
        Reserved2 = Instruction.MarkerByte2,
        Reserved3 = Instruction.MarkerByte3,
        Reserved4 = Instruction.MarkerByte4,
        Reserved5 = Instruction.MarkerByte5,
        Return = (Instruction.MarkerByte1 << 8) | 1,
        //ExampleOfExtendedOpecode = (Instruction.MarkerByte1 << 8) | 1,
    }
}
