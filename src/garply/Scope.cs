using System.Linq;

namespace Garply
{
    internal partial class Scope
    {
        private struct Variable
        {
            public Variable(Value value)
                : this(value, false)
            {
            }
            public Variable(Value value, bool isMutable)
            {
                Value = value;
                IsMutable = isMutable;
            }
            public Value Value { get; }
            public bool IsMutable { get; }
            public override string ToString() => Value.ToString();
        }

        private readonly Variable[] _variables;

        private Scope(Scope other, int size)
            : this(size)
        {
            var errorContext = new ErrorContext();
            for (int i = 0; i < Size && i < other.Size; i++)
            {
                SetValue(errorContext, i, other._variables[i].Value, other._variables[i].IsMutable);
            }
        }

        private Scope(int size)
        {
            _variables = new Variable[size];
        }

        public int Size => _variables.Length;

        public Value GetValue(int index)
        {
            return _variables[index].Value;
        }

        public Value SetValue(ErrorContext errorContext, int index, Value value, bool isMutable)
        {
            if (_variables[index].Value.Type == Types.error) // the variable has never been set
            {
                _variables[index] = new Variable(value, isMutable);
                value.AddRef();
                return value;
            }
            if (_variables[index].IsMutable)
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

        public Scope Copy(int newSize) => new Scope(this, newSize);

        public override string ToString() => $"Scope[{string.Join(", ", _variables.Select(x => x.ToString()))}]";
    }
}
