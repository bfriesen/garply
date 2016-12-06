namespace Garply
{
    public static class Empty
    {
        public static Value List { get; } = new Value(Types.List, 0);
        public static Value Tuple { get; } = new Value(Types.Tuple, 0);
    }
}
