namespace CoordinateLookup.Lookup
{
    public interface IRelativeLookup<T> : IEnumerable<ILookupKeyValuePair<T>>
    {
        public float Next(int count);
        public bool ExpandTo(float distance);
        public HashSet<T> HashSet { get; }
        public float Min { get; }
    }
}
