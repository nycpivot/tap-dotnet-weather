using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Tap.Dotnet.Domain;

namespace Tap.Dotnet.Api.Data
{
    public class WeatherDb : DbContext
    {
        public WeatherDb(DbContextOptions<WeatherDb> options) : base(options)
        {
            //var dbCreator = (RelationalDatabaseCreator)this.Database.GetService<IDatabaseCreator>();

            //if (!dbCreator.HasTables())
            //{
            //    dbCreator.CreateTables();
            //}
        }

        public DbSet<Favorite> Favorites { get; set; }
        //public DbSet<Location> Locations { get; set; }
    }
}
