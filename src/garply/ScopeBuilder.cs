using System.Collections.Generic;

namespace Garply
{
    internal class ScopeBuilder
    {
        private readonly Dictionary<string, int> _indexLookup = new Dictionary<string, int>();
        private int _size;

        public int GetIndex(string variableName)
        {
            int index;
            if (!_indexLookup.TryGetValue(variableName, out index))
            {
                index = _size++;
                _indexLookup.Add(variableName, index);
            }
            return index;
        }

        public int Size { get { return _size; } }
    }
}