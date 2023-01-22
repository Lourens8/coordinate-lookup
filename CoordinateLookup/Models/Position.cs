using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoordinateLookup.Extensions;

namespace CoordinateLookup.Models
{
    [DebuggerDisplay("{PositionId}. {VehicleRegistration} ({Latitude}, {Longitude})")]
    public class Position
    {
        public int PositionId { get; set; }
        public int RegistrationOffset { get; set; }
        public string VehicleRegistration
        {
            get
            {
                //Deferring parsing the vehicle registration trades memory for load time. Could still be optimized
                return DataFileParser.ReadString(RegistrationOffset);
            }
        }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime RecordedTimeUTC
        {
            get
            {
                //Deferring parsing the the date saves a small amount of load time
                return RecordedTime.ToDateTime();
            }
        }
        public ulong RecordedTime { get; set; }
    }
}
