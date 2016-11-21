namespace garply
{
    public static class Types
    {
        static Types()
        {
            Empty = new EmptyType();
            Error = new ErrorType();

            Type = new Type(Empty);

            Name = new Type(Names.Name, Empty);
            Tuple = new Type(Names.Tuple, Empty);
            List = new Type(Names.List, Empty);

            Value = new Type(Names.Value, Empty);
            Boolean = new Type(Names.Boolean, Value);
            String = new Type(Names.String, Value);

            Number = new Type(Names.Number, Value);
            Integer = new Type(Names.Integer, Number);
            Float = new Type(Names.Float, Number);
        }

        public static IType Empty { get; }
        public static IType Error { get; }
        public static Type Type { get; }
        public static Type Name { get; }
        public static Type Tuple { get; }
        public static Type List { get; }
        public static Type Boolean { get; }
        public static Type String { get; }
        public static Type Integer { get; }
        public static Type Float { get; }
        public static Type Number { get; }
        public static Type Value { get; }

        public static void RegisterTo(IMetadataDatabase metadataDatabase)
        {
            //metadataDatabase.RegisterType(Empty);
            //metadataDatabase.RegisterType(Error);
            metadataDatabase.RegisterType(Type);
            metadataDatabase.RegisterType(Name);
            metadataDatabase.RegisterType(Tuple);
            metadataDatabase.RegisterType(List);
            metadataDatabase.RegisterType(Boolean);
            metadataDatabase.RegisterType(String);
            metadataDatabase.RegisterType(Integer);
            metadataDatabase.RegisterType(Float);
            metadataDatabase.RegisterType(Number);
            metadataDatabase.RegisterType(Value);
        }
    }
}
