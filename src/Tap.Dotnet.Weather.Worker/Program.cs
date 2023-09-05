using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Tap.Dotnet.Domain;
using Tap.Dotnet.Weather.Worker;

var connection = System.IO.File.ReadAllText(@"C:\bindings\weather-locations-db\connection");

var optionsBuilder = new DbContextOptionsBuilder<WeatherLocations>();
optionsBuilder.UseNpgsql(connection);

var context = new WeatherLocations(optionsBuilder.Options);

var ass = Assembly.GetExecutingAssembly();

var path = $"{ass.GetName().Name}.Data.uszips.csv";
var stream = new StreamReader(ass.GetManifestResourceStream(path));

var locations = new List<Location>();

var lineNumber = 0;
while (!stream.EndOfStream)
{
    var line = stream.ReadLine();

    if (lineNumber > 0) // skip column headings
    {
        var fields = line.Split(",");

        var zipCode = fields[0];

        var location = new Location()
        {
            ZipCode = fields[0].Replace("\"", "").Replace("\\", ""),
            Latitude = Convert.ToSingle(fields[1].Replace("\"", "").Replace("\\", "")),
            Longitude = Convert.ToSingle(fields[2].Replace("\"", "").Replace("\\", "")),
            CityName = fields[3].Replace("\"", "").Replace("\\", ""),
            StateCode = fields[4].Replace("\"", "").Replace("\\", ""),
            StateName = fields[5].Replace("\"", "").Replace("\\", ""),
            Population = fields[8].Replace("\"", "").Replace("\\", "") == "" ? 0 : Convert.ToInt32(fields[8].Replace("\"", "").Replace("\\", ""))
        };

        Console.WriteLine(fields[0]);

        locations.Add(location);
    }

    lineNumber++;
}

Console.WriteLine();
Console.WriteLine("DONE");
Console.WriteLine();

Console.WriteLine("Enter to continue...");
Console.ReadLine();

foreach(var location in locations.OrderBy(l => l.ZipCode))
{
    Console.WriteLine(location.ZipCode);

    context.Locations.Add(location);
    context.SaveChanges();
}
