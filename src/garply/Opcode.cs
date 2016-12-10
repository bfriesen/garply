namespace Garply
{
    internal enum Opcode : ushort
    {
        Nop,
        GetType,
        TypeIs,
        TypeEquals,
        TupleArity,
        TupleItem,
        ListHead,
        ListTail,
        NewTuple,
        ListEmpty,
        ListAdd,
        LoadBoolean,
        LoadInteger,
        LoadFloat,
        LoadType,
        LoadString,
        AssignVariable,
        ReadVariable,
        Reserved1 = Instruction.MarkerByte1,
        Reserved2 = Instruction.MarkerByte2,
        Reserved3 = Instruction.MarkerByte3,
        Reserved4 = Instruction.MarkerByte4,
        Reserved5 = Instruction.MarkerByte5,
        //ExampleOfExtendedOpcode = (Instruction.MarkerByte1 << 8) | 1,
    }
}
