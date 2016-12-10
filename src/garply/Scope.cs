using System.Linq;

namespace Garply
{
    internal partial class Scope
    {
        private readonly Value[] _variables;

        private Scope(Scope other, int size)
            : this(size)
        {
            var errorContext = new ErrorContext();
            for (int i = 0; i < Size && i < other.Size; i++)
            {
                SetValue(errorContext, i, other._variables[i]);
            }
        }

        private Scope(int size)
        {
            _variables = new Value[size];
        }

        public int Size => _variables.Length;

        public Value GetValue(int index) => _variables[index];

        public Value SetValue(ErrorContext errorContext, Value indexValue, Value value)
        {
            return SetValue(errorContext, (int)indexValue.Raw, value);
        }

        public Value SetValue(ErrorContext errorContext, int index, Value value)
        {
            if (_variables[index].Type != Types.Error)
            {
                errorContext.AddError(new Error("Rebinding is not supported."));
                return default(Value);
            }

            _variables[index] = value;
            value.AddRef();
            return value;
        }

        public void Delete()
        {
            for (int i = 0; i < Size; i++)
            {
                _variables[i].RemoveRef();
            }
        }

        public Scope Copy(int newSize) => new Scope(this, newSize);

        public override string ToString() => $"Scope[{string.Join(", ", _variables.Select(x => x.ToString()))}]";
    }
}
