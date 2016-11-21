namespace garply
{
    public static class Names
    {
        public static IName Empty => Name.Empty;
        public static IName Error => Name.Error;
        public static Name Garply { get; } = new Name("garply");
        public static Name Type { get; } = new Name("type", Garply);
        public static Name Name { get; } = new Name("name", Garply);
        public static Name Tuple { get; } = new Name("tuple", Garply);
        public static Name List { get; } = new Name("list", Garply);
        public static Name Value { get; } = new Name("value", Garply);
        public static Name Boolean { get; } = new Name("boolean", Garply);
        public static Name String { get; } = new Name("string", Garply);
        public static Name Number { get; } = new Name("number", Garply);
        public static Name Integer { get; } = new Name("integer", Garply);
        public static Name Float { get; } = new Name("float", Garply);
    }
}
