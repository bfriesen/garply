using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garply
{
    public static class LongHashCodeExtensions
    {
        private const long _prime = 1251953;
        private const long _stringSeed = 0x542c2caa6d8770ef;

        public static long GetLongHashCode(this string rawValue)
        {
            unchecked
            {
                long hashCode = _stringSeed;
                for (int i = 0; i < rawValue.Length; i++)
                {
                    hashCode = (hashCode * _prime) ^ rawValue[i];
                }
                return hashCode;
            }
        }
    }
}
