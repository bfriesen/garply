using System;

namespace garply
{
    public static class TypedIsExtension
    {
        public static bool Is(this ITyped typed, Type targetType)
        {
#if UNSTABLE
            if (typed == null) throw new ArgumentNullException("typed");
            if (targetType == null) throw new ArgumentNullException("targetType");
#endif
            return typed.Type.Is(targetType);
        }
    }
}
