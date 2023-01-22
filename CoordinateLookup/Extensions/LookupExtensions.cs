using CoordinateLookup.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateLookup.Extensions
{
    public static class LookupExtensions
    {
        public static RelativeLookup<T> ToRelativeLookup<T>(this IEnumerable<T> source, Func<T, float> keySelector)
        {
            return RelativeLookup<T>.Create(source, keySelector);
        }

        public static DistanceLookup<T> ToDistanceLookup<T>(this IEnumerable<T> source, Func<T, float> keySelector1, Func<T, float> keySelector2)
        {
            return DistanceLookup<T>.Create(source, keySelector1, keySelector2);
        }
    }
}
