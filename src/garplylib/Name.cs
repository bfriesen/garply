using System;
using System.Diagnostics;
using System.Text;

namespace Garply
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class Name
    {
        public Name(string value)
        {
            Debug.Assert(value != null);
            Value = value;
            ParentName = this;
        }

        public Name(string value, Name parentName)
        {
            Debug.Assert(value != null);
            Value = value;
            ParentName = parentName;
        }

        public string Value { get; }
        public Name ParentName { get; }
        public bool HasParentName => !ReferenceEquals(this, ParentName);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = 0;
                foreach (var c in Value)
                {
                    hashcode = (hashcode * 397) ^ c;
                }

                hashcode = (hashcode * 397) ^ (HasParentName ? 0 : ParentName.GetHashCode());
                return hashcode;
            }
        }

        public override bool Equals(object obj)
        {
            Debug.Assert(obj != null);
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as Name;
            if (other == null) return false;
            return Equals(other);
        }

        public bool Equals(Name other)
        {
            Debug.Assert(other != null);
            if (!Value.Equals(other.Value, StringComparison.Ordinal)) return false;
            if (!HasParentName) return !other.HasParentName;
            else return ParentName.Equals(other.ParentName);
        }

        internal string DebuggerDisplay
        {
            get
            {
                var sb = new StringBuilder();
                Name name = this;
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
