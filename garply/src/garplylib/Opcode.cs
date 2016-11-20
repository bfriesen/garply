namespace garply
{
    public enum Opcode : ushort
    {
        Nop,
        Reserved1 = Instruction.MarkerByte1,
        Reserved2 = Instruction.MarkerByte2,
        Reserved3 = Instruction.MarkerByte3,
        Reserved4 = Instruction.MarkerByte4,
        Reserved5 = Instruction.MarkerByte5,
        //ExampleOfExtendedOpecode = (Instruction.MarkerByte1 << 8) | 1,
    }
}
