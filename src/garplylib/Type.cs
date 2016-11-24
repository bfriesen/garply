namespace Garply
{
    public enum Types : ushort
    {
        Error
            = 0x00000000,
        Tuple
            = 0x00000001,
        List
            = 0x00000002,
        Type
            = 0x00000004,
        Value
            = 0x00000008,
        Boolean
            = 0x00000010 | Value,
        String
            = 0x00000020 | Value,
        Number
            = 0x00000040 | Value,
        Integer
            = 0x00000080 | Number,
        Float
            = 0x00000100 | Number,
        Operand
            = 0x00000200,
    }
}
