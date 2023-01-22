using CoordinateLookup.Models;
using System.Text;

namespace CoordinateLookup
{
    public class DataFileParser
    {
        public static byte[] Data { get; set; }

        public static List<Position> ReadPositions(string path)
        {
            Data = ReadFile(path);

            var vehiclePositionList = new List<Position>();
            var offset = 0;
            while (offset < Data.Length)
                vehiclePositionList.Add(ReadVehiclePosition(Data, ref offset));

            return vehiclePositionList;
        }

        public static byte[] ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                path = Path.Combine(Environment.CurrentDirectory, "VehiclePositions.dat");
            }

            while (!File.Exists(path))
            {
                Console.WriteLine("File not found");
                Console.WriteLine("Please specify data file path:");
                path = Console.ReadLine();
            }

            return File.ReadAllBytes(path);
        }

        public static string ReadString(int offset)
        {
            var stringBuilder = new StringBuilder();
            while (Data[offset] != 0)
            {
                stringBuilder.Append((char)Data[offset]);
                ++offset;
            }
            return stringBuilder.ToString();
        }

        private static Position ReadVehiclePosition(byte[] data, ref int offset) => CreatePosition(data, ref offset);

        private static Position CreatePosition(byte[] buffer, ref int offset)
        {
            var vehiclePosition = new Position();
            vehiclePosition.PositionId = BitConverter.ToInt32(buffer, offset);

            offset += 4;

            vehiclePosition.RegistrationOffset = offset;

            while (buffer[offset] != 0)
            {
                ++offset;
            }

            ++offset;
            vehiclePosition.Latitude = BitConverter.ToSingle(buffer, offset);
            offset += 4;
            vehiclePosition.Longitude = BitConverter.ToSingle(buffer, offset);
            offset += 4;
            var uint64 = BitConverter.ToUInt64(buffer, offset);
            vehiclePosition.RecordedTime = uint64;
            offset += 8;
            return vehiclePosition;
        }
    }
}
