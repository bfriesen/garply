using System;
using System.Diagnostics;
using System.IO;

namespace garply
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class String : IFirstClassType, IOperand
    {
        private String(IType type)
        {
            Value = "";
            Type = type;
        }

        public String(string value)
        {
#if UNSTABLE
            if (value == null) throw new ArgumentNullException("value");
#endif
            Value = value;
            Type = value == "" ? Types.Empty : Types.String;
        }

        public static String Empty { get; } = new String(Types.Empty);
        public static String Error { get; } = new String(Types.Error);

        public IType Type { get; }

        public string Value { get; }

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            var id = metadataDatabase.GetStringId(this);
            id.Write(writer, metadataDatabase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            var other = obj as String;
            if (other == null) return false;
            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = 0;
                foreach (var c in Value)
                {
                    hashcode = (hashcode * 397) ^ c;
                }
                return hashcode;
            }
        }

        internal string DebuggerDisplay
        {
            get
            {
                if (Type.Equals(Types.Empty))
                {
                    return "empty<string>";
                }
                else if (Type.Equals(Types.Error))
                {
                    return "error<string>";
                }
                else
                {
                    return Value.ToString();
                }
            }
        }
    }
}
