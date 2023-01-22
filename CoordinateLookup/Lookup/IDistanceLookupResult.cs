using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateLookup.Lookup
{
    public interface IDistanceLookupResult<T> : IEnumerable<T>
    {
        public IEnumerable<T> Take(int count);
    }
}
