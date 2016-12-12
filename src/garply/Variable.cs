namespace Garply
{
    internal struct Variable
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
        public override string ToString() => Value.Type == Types.expression
            ? Heap.GetExpression((int)Value.Raw).ToString(true)
            : Value.ToString();
    }
}
