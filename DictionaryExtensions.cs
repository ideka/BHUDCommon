using System.Collections.Generic;

namespace Ideka.BHUDCommon
{
    internal static class DictionaryExtensions
    {
        public static void Deconstruct<TK, TV>(this KeyValuePair<TK, TV> pair, out TK key, out TV value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
