namespace Garply
{
    internal static class Empty
    {
        public static Value List { get; } = new Value(Types.list, 0);
        public static Value Tuple { get; } = new Value(Types.tuple, 0);
    }
}
