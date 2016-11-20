using System;
using System.IO;

namespace garply
{
    public class String : ITyped, IOperand
    {
        public String(string value)
        {
#if UNSTABLE
            if (value == null) throw new ArgumentNullException("value");
#endif
            Value = value;
        }

        public Type Type => Type.StringType;

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
            return Value.GetHashCode();
        }
    }
}
