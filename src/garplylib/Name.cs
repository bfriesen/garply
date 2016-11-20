using System;

namespace garply
{
    public sealed class Name
    {
        private static readonly Name GarplyName = new Name("garply", null);
        internal static readonly Name TypeName = new Name("type", GarplyName);
        internal static readonly Name TupleName = new Name("tuple", GarplyName);
        internal static readonly Name ListName = new Name("list", GarplyName);
        internal static readonly Name BooleanName = new Name("boolean", GarplyName);
        internal static readonly Name StringName = new Name("string", GarplyName);
        internal static readonly Name IntegerName = new Name("integer", GarplyName);
        internal static readonly Name FloatName = new Name("float", GarplyName);
        internal static readonly Name NumberName = new Name("number", GarplyName);
        internal static readonly Name ValueName = new Name("value", GarplyName);

        public Name(string value, Name parentName = null)
        {
#if UNSTABLE
            if (value == null) throw new ArgumentNullException("value");
#endif
            Value = value;
            ParentName = parentName;
        }

        public string Value { get; }
        public Name ParentName { get; }

        public override int GetHashCode()
        {
            return unchecked((Value.GetHashCode() * 397) ^ (ParentName == null ? 0 : ParentName.GetHashCode()));
        }

        public override bool Equals(object obj)
        {
#if UNSTABLE
            if (obj == null) throw new ArgumentNullException("obj");
#endif
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as Name;
            if (other == null) return false;
            return Equals(other);
        }

        public bool Equals(Name other)
        {
#if UNSTABLE
            if (other == null) throw new ArgumentNullException("other");
#endif
            if (!Value.Equals(other.Value, StringComparison.Ordinal)) return false;
            if (ParentName == null) return other.ParentName == null;
            else return ParentName.Equals(other.ParentName);
        }
    }
}
