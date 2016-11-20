using System;

namespace garply
{
    public class String : ITyped
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
    }
}
