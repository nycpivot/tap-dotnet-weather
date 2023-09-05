using Microsoft.EntityFrameworkCore;
using Tap.Dotnet.Domain;

namespace Tap.Dotnet.Weather.Worker
{
    internal class WeatherLocations : DbContext
    {
        public WeatherLocations(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Location> Locations { get; set; }
    }
}
