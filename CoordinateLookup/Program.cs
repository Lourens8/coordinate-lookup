using CoordinateLookup;
using CoordinateLookup.Extensions;
using CoordinateLookup.Models;
using System.Diagnostics;

var path = "C:\\Projects\\CoordinateLookup\\VehiclePositions.dat";

var s = Stopwatch.StartNew();
var items = DataFileParser.ReadPositions(path);
s.Stop();

var loadTime = s.ElapsedMilliseconds;
Console.WriteLine($"Data file read time : {loadTime} ms");

s.Restart();

var lookup = items.ToDistanceLookup(t => t.Latitude, t => t.Longitude);

s.Stop();

var indexTime = s.ElapsedMilliseconds;
Console.WriteLine($"Index time : {indexTime} ms");

s.Restart();

foreach (var item in TestData.Positions)
{
    var result = lookup[(item.Latitude, item.Longitude)].Take(1);
    Console.WriteLine($"{result.FirstOrDefault().VehicleRegistration}");
}

s.Stop();
var lookupTime = s.ElapsedMilliseconds;
Console.WriteLine($"Lookup time : {lookupTime} ms");
Console.WriteLine($"Total time : {lookupTime + indexTime + loadTime} ms");

Console.ReadLine();