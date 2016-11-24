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
                case Opcode.PushArg:
                case Opcode.Return:
                case Opcode.GetType:
                case Opcode.TypeIs:
                case Opcode.ListEmpty:
                case Opcode.ListAdd:
                //case Opcode.TypeEquals:
                    return 0;
                case Opcode.LoadBoolean:
                case Opcode.TupleItem:
                case Opcode.NewTuple:
                    return 1;
                case Opcode.LoadType:
                    return 4;
                case Opcode.LoadString:
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
