using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateLookup.Extensions
{
    public static class LongExtensions
    {
        public static DateTime UnixBase = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static DateTime ToDateTime(this ulong seconds)
        {
            return UnixBase.AddSeconds(seconds);
        }
    }
}
