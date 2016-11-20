using System;

namespace garply
{
    public static class OpcodeGetSizeExtension
    {
        public static int GetSize(this Opcode opcode)
        {
            switch (opcode)
            {
                case Opcode.Nop:
                case Opcode.Return:
                    return 0;
                case Opcode.LoadBoolean:
                    return 1;
                case Opcode.LoadString:
                case Opcode.LoadInteger:
                case Opcode.LoadFloat:
                case Opcode.LoadType:
                    return 8;
                case Opcode.Reserved1:
                case Opcode.Reserved2:
                case Opcode.Reserved3:
                case Opcode.Reserved4:
                case Opcode.Reserved5:
                    // TODO: Log a warning - the size of a reserved opcode should never be needed.
                    return 0;
                default:
                    throw new ArgumentException($"Invalid opcode value: {opcode}", "opcode");
            }
        }
    }
}
