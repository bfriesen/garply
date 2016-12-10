using System.Collections.Concurrent;

namespace Garply
{
    internal static class StringDatabase
    {
        private static readonly ConcurrentDictionary<long, string> _strings = new ConcurrentDictionary<long, string>();

        public static Value Register(string rawValue)
        {
            var key = rawValue.GetLongHashCode();
            _strings.TryAdd(key, rawValue);
            return new Value(Types.@string, key);
        }

        public static string GetRawValue(long id)
        {
            return _strings[id];
        }
    }
}
