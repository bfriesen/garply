using System.Collections.Generic;
using System.Text;

namespace Garply
{
    internal struct Tuple
    {
        private readonly Value[] _items;

        internal Tuple(params Value[] items)
        {
            _items = items;
        }

        public bool IsEmpty => _items == null || _items.Length == 0;

        public IReadOnlyList<Value> Items => _items;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('(');
            if (_items != null)
            {
                for (int i = 0; i < _items.Length; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(_items[i].ToString());
                }
            }
            sb.Append(')');
            return sb.ToString();
        }
    }
}
