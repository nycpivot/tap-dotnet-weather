using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Immutable;
using System.Net;
using Tap.Dotnet.Weather.Api.Interfaces;
using Tap.Dotnet.Weather.Domain;
using Wavefront.SDK.CSharp.Common;

namespace Tap.Dotnet.Weather.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IWeatherDataService weatherDataService;
        private readonly IWavefrontSender wavefrontSender;
        private readonly ILogger<ForecastController> logger;

        public FavoritesController(
            IWeatherDataService weatherDataService,
            IWavefrontSender wavefrontSender,
            ILogger<ForecastController> logger)
        {
            this.weatherDataService = weatherDataService;
            this.wavefrontSender = wavefrontSender;
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<Favorite> Get()
        {
            var favorites = new List<Favorite>();

            var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            Thread.Sleep(100);
            var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            var traceId = this.Request.Headers["X-TraceId"][0];
            var spanId = this.Request.Headers["X-SpanId"][0];

            this.wavefrontSender.SendSpan(
                "Get", start, end, "WeatherApi", new Guid(traceId), Guid.NewGuid(),
                ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                ImmutableList.Create(
                    new KeyValuePair<string, string>("application", "tap-dotnet-weather-api"),
                    new KeyValuePair<string, string>("service", "WeatherApi.FavoritesController"),
                    new KeyValuePair<string, string>("http.method", "GET")), null);

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherDataService.Url);
                    httpClient.DefaultRequestHeaders.Add("X-TraceId", traceId.ToString());
                    httpClient.DefaultRequestHeaders.Add("X-SpanId", spanId.ToString());

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

            var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            Thread.Sleep(100);
            var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            var traceId = this.Request.Headers["X-TraceId"][0];
            var spanId = this.Request.Headers["X-SpanId"][0];

            this.wavefrontSender.SendSpan(
                "Save", start, end, "WeatherApi", new Guid(traceId), Guid.NewGuid(),
                ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                ImmutableList.Create(
                    new KeyValuePair<string, string>("application", "tap-dotnet-weather-api"),
                    new KeyValuePair<string, string>("service", "WeatherApi.FavoritesController"),
                    new KeyValuePair<string, string>("zipcode", zipCode),
                    new KeyValuePair<string, string>("http.method", "POST")), null);

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherDataService.Url);
                    httpClient.DefaultRequestHeaders.Add("X-TraceId", traceId.ToString());
                    httpClient.DefaultRequestHeaders.Add("X-SpanId", spanId.ToString());

                    var result = httpClient.GetAsync($"favorites/{zipCode}").Result;
                }
            }
        }
    }
}
