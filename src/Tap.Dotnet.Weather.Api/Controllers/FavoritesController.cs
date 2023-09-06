using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using Tap.Dotnet.Weather.Api.Interfaces;
using Tap.Dotnet.Weather.Domain;

namespace Tap.Dotnet.Weather.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IWeatherDataService weatherDataService;
        private readonly ILogger<ForecastController> logger;

        public FavoritesController(IWeatherDataService weatherDataService, ILogger<ForecastController> logger)
        {
            this.weatherDataService = weatherDataService;
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<Favorite> Get()
        {
            var favorites = new List<Favorite>();

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherDataService.Url);

                    var response = httpClient.GetAsync($"favorites").Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;

                        favorites = JsonConvert.DeserializeObject<IList<Favorite>>(content).ToList();

                        foreach (var favorite in favorites)
                        {
                            //favorite.CityName = "";
                            //favorite.StateCode = "";
                            //favorite.CountryCode = "";
                        }    
                    }
                }
            }

            return favorites;
        }

        [HttpGet]
        [Route("{zipCode}")]
        public void Save(string zipCode)
        {
            var favorites = new List<Favorite>();

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherDataService.Url);

                    var result = httpClient.GetAsync($"favorites/{zipCode}").Result;
                }
            }
        }
    }
}
