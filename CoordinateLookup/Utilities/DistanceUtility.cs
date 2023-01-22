using GeoCoordinatePortable;

namespace CoordinateLookup.Utilities
{
    public class DistanceUtility
    {
        internal static double CalculateDistance(float lat1, float long1, float lat2, float long2) => new GeoCoordinate(lat1, long1).GetDistanceTo(new GeoCoordinate(lat2, long2));
    }
}
