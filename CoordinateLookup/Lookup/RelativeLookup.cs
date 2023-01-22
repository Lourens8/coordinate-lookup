using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoordinateLookup.Lookup
{
    /// <summary>
    /// Finds elements relative to a key
    /// </summary>
    public class RelativeLookup<T>
    {
        public readonly Func<T, float> _keySelector;

        private List<KeyValuePair> _items;

        private void Initialize(IEnumerable<KeyValuePair> items) => _items = new List<KeyValuePair>(items);

        public RelativeLookup(Func<T, float> keySelector)
        {
            _keySelector = keySelector;
        }

        public static RelativeLookup<T> Create(IEnumerable<T> items, Func<T, float> keySelector)
        {
            var lookup = new RelativeLookup<T>(keySelector);

            var s = Stopwatch.StartNew();

            IEnumerable<KeyValuePair> elements = new List<KeyValuePair>();
            var t1 = Task.Run(() => elements = items
                                                .AsParallel()
                                                .OrderBy(t => keySelector(t))
                                                .Select((t, index) => KeyValuePair.Create(lookup, t, index))
                                                .ToList());

            t1.Wait();

            lookup.Initialize(elements);

            s.Stop();

            return lookup;
        }

        public int Count => _items.Count;

        public IRelativeLookup<T> this[float key] => new Result(this, key);

        [DebuggerDisplay("{Value}", Name = "{Key}")]
        private readonly struct KeyValuePair : ILookupKeyValuePair<T>, IComparable<KeyValuePair>
        {
            private readonly RelativeLookup<T> _lookup;
            private readonly T _value;
            private readonly int _keyStartIndex;

            public float Key => _lookup._keySelector(_value);

            public T Value => _value;

            public int KeyStartIndex => _keyStartIndex;

            private KeyValuePair(RelativeLookup<T> lookup, T value, int keyStartIndex)
            {
                _lookup = lookup;
                _value = value;
                _keyStartIndex = keyStartIndex;
            }

            public int CompareTo(KeyValuePair other)
            {
                return _lookup._keySelector(Value).CompareTo(_lookup._keySelector(other.Value));
            }

            public override bool Equals(object? obj)
            {
                if (obj is KeyValuePair kvp)
                {
                    return CompareTo(kvp) == 0;
                }

                return false;
            }

            internal static KeyValuePair Create(RelativeLookup<T> lookup, T value, int keyStartIndex) => new KeyValuePair(lookup, value, keyStartIndex);
        }

        private class Result : IRelativeLookup<T>
        {
            private readonly RelativeLookup<T> _lookup;
            private readonly float _key;
            private IList<Range> _rangeCache;
            private int _min;
            private int _max;

            public HashSet<T> HashSet { get; set; }
            public float Min { get; set; }

            public Result(RelativeLookup<T> lookup, float key)
            {
                _lookup = lookup;
                _key = key;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<ILookupKeyValuePair<T>> GetEnumerator() => Find().GetEnumerator();

            private IEnumerable<ILookupKeyValuePair<T>> Find()
            {
                if (_rangeCache == null)
                {
                    _rangeCache = FindRanges().OrderBy(t => t.StartIndex).ToList();
                }

                foreach (var range in _rangeCache)
                {
                    for (var i = range.StartIndex; i < range.EndIndex; i++)
                    {
                        yield return _lookup._items[i];
                    }
                }
            }

            /// <summary>
            /// Expands the results to a set count
            /// </summary>
            public float Next(int count)
            {
                if (_rangeCache == null)
                {
                    HashSet = Find().Select(t => t.Value).ToHashSet();
                }

                var minStart = _min;
                var maxEnd = _max;

                var half = count / 2;

                var item = _lookup._items[minStart - half];
                var distance = _key - _lookup._keySelector(item.Value);
                Min = item.Key;

                ExpandTo(distance);

                return distance;
            }

            /// <summary>
            /// Expands the results to a matching distance
            /// </summary>
            public bool ExpandTo(float distance)
            {
                if (_rangeCache == null)
                {
                    HashSet = Find().Select(t => t.Value).ToHashSet();
                }

                var maxDistance = 0f;
                var minStart = _min;
                var maxEnd = _max;

                while (maxDistance < distance)
                {
                    var diffMax = _lookup._keySelector(_lookup._items[maxEnd + 1].Value) - _key;
                    var diffMin = _key - _lookup._keySelector(_lookup._items[minStart - 1].Value);

                    if (diffMax <= distance)
                    {
                        maxEnd++;
                        HashSet.Add(_lookup._items[maxEnd].Value);
                    }

                    if (diffMin <= distance)
                    {
                        minStart--;
                        HashSet.Add(_lookup._items[minStart].Value);
                    }

                    if (diffMin > maxDistance)
                        maxDistance = diffMin;

                    if (diffMax > maxDistance)
                        maxDistance = diffMin;
                }

                _min = minStart;
                _max = maxEnd;

                return true;
            }

            /// <summary>
            /// Finds matching ranges. Method adapted from standard lookups. Should probably be removed
            /// </summary>
            private IEnumerable<Range> FindRanges()
            {
                var midIndex = FindAnyByKey(_key, 0, _lookup._items.Count - 1, out var min, out var max);

                yield return new Range(midIndex, midIndex + 1);

                _min = midIndex;
                _max = midIndex;
            }

            /// <summary>
            /// Finds any element with the matching key, depending on where the binary lookup lands
            /// </summary>
            private int FindAnyByKey(float key, int start, int end, out int min, out int max)
            {
                min = start;
                max = end;

                while (max >= min)
                {
                    var mid = (min + max) / 2;
                    var result = _lookup._keySelector(_lookup._items[mid].Value).CompareTo(key);

                    if (result < 0)
                    {
                        //Results are after this item
                        min = mid + 1;
                        continue;
                    }

                    if (result > 0)
                    {
                        //Results are before this item
                        max = mid - 1;
                        continue;
                    }

                    return mid;
                }

                return max;
            }

            private readonly struct Range
            {
                private readonly int _startIndex;

                private readonly int _endIndex;

                public int StartIndex => _startIndex;
                public int EndIndex => _endIndex;

                public Range(int startIndex, int endIndex) : this()
                {
                    _startIndex = startIndex;
                    _endIndex = endIndex;
                }
            }
        }
    }
}
