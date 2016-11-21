using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace garply
{
    [DebuggerDisplay("<{DebuggerDisplay,nq}>")]
    public class Type : IType, IOperand
    {
        private readonly IType _type;

        /// <summary>
        /// Constructor used to initialize the Type type.
        /// </summary>
        internal Type(IType emptyType)
        {
            _type = this;
            Name = Names.Type;
            BaseType = emptyType;
        }

        public Type(Name name, IType baseType)
        {
#if UNSTABLE
            if (name == null) throw new ArgumentNullException("name");
            if (baseType == null) throw new ArgumentNullException("baseType");
#endif
            _type = Types.Type;
            Name = name;
            BaseType = baseType;
        }

        public static IType Empty => Types.Empty;
        public static IType Error => Types.Error;

        public IName Name { get; }
        public IType BaseType { get; }
        IType IFirstClassType.Type { get { return _type; } }

        public void Write(BinaryWriter writer, IMetadataDatabase metadataDatabase)
        {
            var id = metadataDatabase.GetTypeId(this);
            id.Write(writer, metadataDatabase);
        }

        public Boolean Is(IType other)
        {
#if UNSTABLE
            if (other == null) throw new ArgumentNullException("other");
#endif
            if (Equals(other)) return Boolean.True;
            if (BaseType.Equals(Types.Empty)) return Boolean.False;
            return BaseType.Is(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashcode = Name.GetHashCode();
                if (BaseType.Type.Equals(Types.Empty))
                {
                    hashcode = (hashcode * 397) ^ 0;
                }
                else
                {
                    hashcode = (hashcode * 397) ^ BaseType.GetHashCode();
                }
                return hashcode;
            }
        }

        public override bool Equals(object obj)
        {
#if UNSTABLE
            if (obj == null) throw new ArgumentNullException("other");
#endif
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as Type;
            if (other == null) return false;
            return Equals(other);
        }

        public bool Equals(Type other)
        {
#if UNSTABLE
            if (other == null) throw new ArgumentNullException("other");
#endif
            if (!Name.Equals(other.Name)) return false;
            if (BaseType.Equals(Types.Empty)) return other.BaseType.Equals(Types.Empty);
            return BaseType.Equals(other.BaseType);
        }

        internal string DebuggerDisplay
        {
            get
            {
                var sb = new StringBuilder();
                IName name = Name;
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
