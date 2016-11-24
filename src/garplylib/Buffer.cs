using System;
using System.Threading;
using BufferTable=System.Runtime.CompilerServices.ConditionalWeakTable<System.Threading.Thread, byte[]>;

namespace Garply
{
    internal static class Buffer
    {
        private static readonly byte[] _zero = new byte[0];
        private static readonly BufferTable _one = new BufferTable();
        private static readonly BufferTable _two = new BufferTable();
        private static readonly BufferTable _four = new BufferTable();
        private static readonly BufferTable _eight = new BufferTable();

        public static byte[] Get(int size)
        {
            BufferTable table;
            switch (size)
            {
                case 0: return _zero;
                case 1: table = _one; break;
                case 2: table = _two; break;
                case 4: table = _four; break;
                case 8: table = _eight; break;
                default: throw new ArgumentOutOfRangeException("size", "Valid values for 'size' are 0, 1, 2, 4, and 8.");
            }
            return table.GetValue(Thread.CurrentThread, key => new byte[size]);
        }
    }
}
