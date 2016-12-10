using System.Collections.Generic;

namespace Garply
{
    internal partial class Scope
    {
        public class Builder
        {
            private readonly Dictionary<string, int> _indexLookup = new Dictionary<string, int>();
            private int _size;

            public int GetOrCreateIndex(string variableName)
            {
                int index;
                if (!_indexLookup.TryGetValue(variableName, out index))
                {
                    index = _size++;
                    _indexLookup.Add(variableName, index);
                }
                return index;
            }

            public bool TryGetIndex(string variableName, out int index)
            {
                return _indexLookup.TryGetValue(variableName, out index);
            }

            public int Size { get { return _size; } }

            public Scope Build()
            {
                return new Scope(_size);
            }
        }
    }
}
