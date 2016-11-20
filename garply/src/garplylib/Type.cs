using System;

namespace garply
{
    public sealed class Type : ITyped
    {
        public static Type TypeType { get; } = new Type(Name.TypeName);
        public static Type TupleType { get; } = new Type(Name.TupleName);
        public static Type ListType { get; } = new Type(Name.ListName);
        public static Type BooleanType { get; } = new Type(Name.BooleanName, ValueType);
        public static Type StringType { get; } = new Type(Name.StringName, ValueType);
        public static Type IntegerType { get; } = new Type(Name.IntegerName, NumberType);
        public static Type FloatType { get; } = new Type(Name.FloatName, NumberType);
        public static Type NumberType { get; } = new Type(Name.NumberName, ValueType);
        public static Type ValueType { get; } = new Type(Name.ValueName);

        public Type(Name name, Type baseType = null)
        {
#if UNSTABLE
            if (name == null) throw new ArgumentNullException("name");
#endif
            Name = name;
            BaseType = baseType;
        }

        public Name Name { get; }
        public Type BaseType { get; }

        Type ITyped.Type => TypeType;

        public bool Is(Type other)
        {
#if UNSTABLE
            if (other == null) throw new ArgumentNullException("other");
#endif
            if (Equals(other)) return true;
            if (BaseType == null) return false;
            return BaseType.Is(other);
        }

        public bool Equals(Type other)
        {
#if UNSTABLE
            if (other == null) throw new ArgumentNullException("other");
#endif
            if (!Name.Equals(other.Name)) return false;
            if (BaseType == null) return other.BaseType == null;
            return BaseType.Equals(other.BaseType);
        }
    }
}
