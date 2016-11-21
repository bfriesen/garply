using System;

namespace garply
{
    public static class FirstClassTypeIsExtension
    {
        public static bool Is(this IFirstClassType value, IType targetType)
        {
#if UNSTABLE
            if (value == null) throw new ArgumentNullException("typed");
            if (targetType == null) throw new ArgumentNullException("targetType");
#endif
            return value.Type.Is(targetType);
        }
    }
}
