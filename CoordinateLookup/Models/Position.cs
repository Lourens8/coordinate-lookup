using CoordinateLookup.Extensions;
using System.Diagnostics;

namespace CoordinateLookup.Models
{
    [DebuggerDisplay("{PositionId}. {VehicleRegistration} ({Latitude}, {Longitude})")]
    public class Position
    {
        public int PositionId { get; set; }
        public int RegistrationOffset { get; set; }
        public string VehicleRegistration =>
                //Deferring parsing the vehicle registration trades memory for load time. Could still be optimized
                DataFileParser.ReadString(RegistrationOffset);

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime RecordedTimeUTC =>
                //Deferring parsing the the date saves a small amount of load time
                RecordedTime.ToDateTime();

        public ulong RecordedTime { get; set; }
    }
}
