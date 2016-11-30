using System.Collections.Generic;
using System.Diagnostics;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Tuple
    {
        private readonly Value[] _items;

        internal Tuple(params Value[] items)
        {
            _items = items;
        }

        public IReadOnlyList<Value> Items => _items;

        internal string DebuggerDisplay => $"tuple({_items.Length})";
    }
}
