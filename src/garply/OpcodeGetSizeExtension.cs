using System;

namespace Garply
{
    public static class OpcodeGetSizeExtension
    {
        public static int GetSize(this Opcode opcode)
        {
            switch (opcode)
            {
                case Opcode.Nop:
                case Opcode.GetType:
                case Opcode.TypeIs:
                case Opcode.TypeEquals:
                case Opcode.ListEmpty:
                case Opcode.ListAdd:
                case Opcode.ListHead:
                case Opcode.ListTail:
                case Opcode.TupleArity:
                    return 0;
                case Opcode.LoadBoolean:
                case Opcode.TupleItem:
                case Opcode.NewTuple:
                    return 1;
                case Opcode.LoadType:
                case Opcode.LoadString:
                    return 4;
                case Opcode.LoadInteger:
                case Opcode.LoadFloat:
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
