using System.Collections.Generic;
using System.Linq;

namespace Garply
{
    internal partial class Scope
    {
        private readonly Variable[] _variables;
        private readonly string[] _variableNames;

        private Scope(Scope other, Dictionary<string, int> variableDefinitions)
            : this(variableDefinitions)
        {
            var errorContext = new ErrorContext();
            for (int i = 0; i < Size && i < other.Size; i++)
            {
                SetValue(errorContext, i, other._variables[i].Value, other._variables[i].IsMutable);
            }
        }

        private Scope(Dictionary<string, int> variableDefinitions)
        {
            _variables = new Variable[variableDefinitions.Count];
            _variableNames = new string[variableDefinitions.Count];
            foreach (var variableDefinition in variableDefinitions)
            {
                _variableNames[variableDefinition.Value] = variableDefinition.Key;
            }
        }

        public int Size => _variables.Length;

        public Value GetValue(int index)
        {
            return _variables[index].Value;
        }

        public Value    SetValue(ErrorContext errorContext, int index, Value value, bool allowMutability)
        {
            if (_variables[index].Value.Type == Types.error) // the variable has never been set
            {
                _variables[index] = new Variable(value, allowMutability);
                value.AddRef();
                return value;
            }
            if (allowMutability && _variables[index].IsMutable)
            {
                _variables[index].Value.RemoveRef();
                _variables[index] = new Variable(value, _variables[index].IsMutable);
                value.AddRef();
                return value;
            }
            errorContext.AddError(new Error("Cannot rebind to immutable variable."));
            return default(Value);
        }

        public void Delete()
        {
            for (int i = 0; i < Size; i++)
            {
                _variables[i].Value.RemoveRef();
            }
        }

        public Scope Copy(Builder scopeBuilder) => new Scope(this, scopeBuilder.GetVariableDefinitions());

        public override string ToString() => _variables.Length == 0
            ? "Scope[]"
            : $"Scope[\r\n  {string.Join(",\r\n  ", _variables.Zip(_variableNames, (v, n) => $"{{{n},{(v.Value.Type == Types.error ? "" : v.ToString())}}}"))}\r\n]";
    }
}
