namespace CoordinateLookup.Lookup
{
    public interface ILookupKeyValuePair<out T>
    {
        float Key { get; }
        T Value { get; }
        int KeyStartIndex { get; }
    }
}
