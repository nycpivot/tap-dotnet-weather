using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Tap.Dotnet.Weather.Domain;
using Wavefront.SDK.CSharp.Common;

namespace Tap.Dotnet.Weather.Data.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly WeatherDb weatherDb;
        private readonly IWavefrontSender wavefrontSender;
        private readonly ILogger<FavoritesController> logger;

        public FavoritesController(
            WeatherDb weatherDb, IWavefrontSender wavefrontSender,
            ILogger<FavoritesController> logger)
        {
            this.weatherDb = weatherDb;
            this.wavefrontSender = wavefrontSender;
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<Favorite> Get()
        {
            if (this.Request.Headers.ContainsKey("X-TraceId"))
            {
                var traceId = this.Request.Headers["X-TraceId"][0];
                var spanId = this.Request.Headers["X-SpanId"][0];

                this.wavefrontSender.SendSpan(
                    "Get", 0, 1, "WeatherData", new Guid(traceId), Guid.NewGuid(),
                    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                    ImmutableList.Create(
                        new KeyValuePair<string, string>("application", "tap-dotnet-weather-data"),
                        new KeyValuePair<string, string>("service", "WeatherData.FavoritesController"),
                        new KeyValuePair<string, string>("http.method", "GET")), null);
            }

            var favorites = this.weatherDb.Favorites;

            return favorites;
        }

        [HttpGet]
        [Route("{zipCode}")]
        public void Save(string zipCode)
        {
            if (this.Request.Headers.ContainsKey("X-TraceId"))
            {
                var traceId = this.Request.Headers["X-TraceId"][0];
                var spanId = this.Request.Headers["X-SpanId"][0];

                this.wavefrontSender.SendSpan(
                    "Save", 0, 1, "WeatherData", new Guid(traceId), Guid.NewGuid(),
                    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                    ImmutableList.Create(
                        new KeyValuePair<string, string>("application", "tap-dotnet-weather-data"),
                        new KeyValuePair<string, string>("service", "WeatherData.FavoritesController"),
                        new KeyValuePair<string, string>("http.method", "POST")), null);
            }

            var favorite = new Favorite();
            favorite.ZipCode = zipCode;

            try
            {
                this.weatherDb.Favorites.Add(favorite);
                this.weatherDb.SaveChanges();
            }
            catch(DbUpdateException ex)
            {

            }
        }
    }
}
