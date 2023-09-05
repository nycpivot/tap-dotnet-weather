using Microsoft.AspNetCore.Mvc;
using Tap.Dotnet.Domain;
using Wavefront.SDK.CSharp.Common;

namespace Tap.Dotnet.Api.Data.Controllers
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
            var favorites = this.weatherDb.Favorites;

            return favorites;
        }

        [HttpGet]
        [Route("{zipCode}")]
        public void Save(string zipCode)
        {
            var favorite = new Favorite();
            favorite.ZipCode = zipCode;

            this.weatherDb.Favorites.Add(favorite);
            this.weatherDb.SaveChanges();
        }
    }
}
