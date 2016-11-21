using System;

namespace garply
{
    public abstract class EmptyBase
    {
        public virtual IType Type => Types.Empty;

        public override int GetHashCode()
        {
            return 985569825;
        }

        public override bool Equals(object obj)
        {
#if UNSTABLE
            if (ReferenceEquals(obj, null)) throw new ArgumentNullException("obj");
#endif            
            if (ReferenceEquals(this, obj)) return true;
            var other = obj as IFirstClassType;
            if (other == null) return false;
            return Type.Name.Equals(other.Type.Name);
        }
    }
}
