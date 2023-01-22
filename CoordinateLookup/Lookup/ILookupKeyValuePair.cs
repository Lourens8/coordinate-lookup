using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateLookup.Lookup
{
    public interface ILookupKeyValuePair<out T>
    {
        float Key { get; }
        T Value { get; }
        int KeyStartIndex { get; }
    }
}
