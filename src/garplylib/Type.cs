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

        /// <summary>
        /// Constructor used to initialize the Empty and Error types.
        /// </summary>
        /// <param name="name">Either Empty or Error.</param>
        internal Type(Name name)
        {
#if UNSTABLE
            if (name == null) throw new ArgumentNullException("name");
            if (!(name.ParentName.Value == "garply"
                && name.ParentName.ParentName.Value == ""
                && (name.Value.Equals("empty") || name.Value.Equals("error"))))
                    throw new ArgumentException("The 'name' parameter must be Empty or Error.", "name");
#endif
            _type = this;
            Name = name;
            BaseType = this;
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

        public bool Is(IType other)
        {
#if UNSTABLE
            if (other == null) throw new ArgumentNullException("other");
#endif
            if (Equals(other)) return true;
            if (BaseType == null) return false;
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
            if (BaseType == null) return other.BaseType == null;
            return BaseType.Equals(other.BaseType);
        }

        private string DebuggerDisplay
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
