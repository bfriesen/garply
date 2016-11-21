using System;
using System.Diagnostics;
using System.Text;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class Name : IName
    {
        private readonly Lazy<IType> _type;

        public Name(string value)
            : this(value, new EmptyName())
        {
        }

        public Name(string value, IName parentName)
        {
#if UNSTABLE
            if (value == null) throw new ArgumentNullException("value");
#endif
            Value = value;
            ParentName = parentName;
        }

        public static IName Empty { get; } = new EmptyName();
        public static IName Error { get; } = new ErrorName();

        public IType Type => Types.Name;
        public string Value { get; }
        public IName ParentName { get; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = Value.GetHashCode();
                if (ParentName.Type.Equals(Types.Empty))
                {
                    hashcode = (hashcode * 397) ^ 0;
                }
                else
                {
                    hashcode = (hashcode * 397) ^ ParentName.GetHashCode();
                }
                return hashcode;
            }
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

        private string DebuggerDisplay
        {
            get
            {
                var sb = new StringBuilder();
                IName name = this;
                while (name is Name)
                {
                    if (sb.Length > 0) sb.Insert(0, '.');
                    sb.Insert(0, name.Value);
                    name = name.ParentName;
                }
                return sb.ToString();
            }
        }
    }
}
