using System.Collections.Generic;

namespace Garply
{
    internal partial class Scope
    {
        public class Builder
        {
            private readonly Dictionary<string, int> _variables = new Dictionary<string, int>();
            private int _size;

            public int GetOrCreateIndex(string variableName)
            {
                int variableIndex;
                if (!_variables.TryGetValue(variableName, out variableIndex))
                {
                    variableIndex = _size++;
                    _variables.Add(variableName, variableIndex);
                }
                return variableIndex;
            }

            public bool TryGetIndex(string variableName, out int index)
            {
                return _variables.TryGetValue(variableName, out index);
            }

            public int Size { get { return _size; } }

            public Scope Build()
            {
                return new Scope(_variables);
            }

            public Dictionary<string, int> GetVariableDefinitions() => _variables;
        }
    }
}
