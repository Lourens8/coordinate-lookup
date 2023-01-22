namespace CoordinateLookup.Lookup
{
    public interface IDistanceLookupResult<T> : IEnumerable<T>
    {
        public IEnumerable<T> Take(int count);
    }
}
