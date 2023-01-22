using CoordinateLookup.Extensions;
using CoordinateLookup.Utilities;
using System.Collections;

namespace CoordinateLookup.Lookup
{
    /// <summary>
    /// Finds relative elements based on distance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DistanceLookup<T>
    {
        private RelativeLookup<T> _l1;
        private RelativeLookup<T> _l2;
        private Func<T, float> _keySelector1;
        private Func<T, float> _keySelector2;

        public DistanceLookup(RelativeLookup<T> l1, RelativeLookup<T> l2, Func<T, float> keySelector1, Func<T, float> keySelector2)
        {
            _l1 = l1;
            _l2 = l2;
            _keySelector1 = keySelector1;
            _keySelector2 = keySelector2;
        }

        public static DistanceLookup<T> Create(IEnumerable<T> items, Func<T, float> keySelector1, Func<T, float> keySelector2)
        {
            RelativeLookup<T> l1 = null, l2 = null;

            //Multi threading shaves some time
            var t1 = Task.Run(() => { l1 = items.ToRelativeLookup(keySelector1); });
            var t2 = Task.Run(() => { l2 = items.ToRelativeLookup(keySelector2); });

            Task.WaitAll(t1, t2);

            return new DistanceLookup<T>(l1, l2, keySelector1, keySelector2);
        }

        public IDistanceLookupResult<T> this[(float key1, float key2) key] => new Result(this, key);

        private class Result : IDistanceLookupResult<T>
        {
            private readonly DistanceLookup<T> _lookup;
            private readonly (float key1, float key2) _key;
            private int _maxExpansion = 100000;

            public Result(DistanceLookup<T> lookup, (float key1, float key2) key)
            {
                _lookup = lookup;
                _key = key;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<T> GetEnumerator() => Take(1).GetEnumerator();

            public IEnumerable<T> Take(int count)
            {
                //1. Find the closest match in both dimensions
                var l1 = _lookup._l1[_key.key1];
                var l2 = _lookup._l2[_key.key2];
                var match = new List<T>();

                while (match.Count <= count)
                {
                    //2. Expand the results to a square containing about 1000 results
                    var distance = l1.Next(1000);
                    l2.ExpandTo(distance);

                    var v1 = l1.HashSet;
                    var v2 = l2.HashSet;

                    if (v1.Count > _maxExpansion)
                        break;

                    //3. Find the overlap between the two sets
                    match = v1.Where(t => v2.Contains(t)).ToList();

                    //4. If there are enough matches to continue, reduce the area to a circle
                    if (match.Count > count)
                    {
                        var maxDistance = DistanceUtility.CalculateDistance(l1.Min, _key.key2, _key.key1, _key.key2);

                        var measured = match.Select(t => new
                        {
                            distance = DistanceUtility.CalculateDistance(_lookup._keySelector1(t),
                                                                         _lookup._keySelector2(t),
                                                                         _key.key1,
                                                                         _key.key2),
                            item = t
                        }).Where(t => t.distance <= maxDistance).ToList();

                        if (measured.Count >= count)
                            return measured.OrderBy(t => t.distance).Take(count).Select(t => t.item).ToList();

                        match = measured.Select(t => t.item).ToList();
                    }
                }

                return null;
            }
        }
    }
}
