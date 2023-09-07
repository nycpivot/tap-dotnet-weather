using App.Metrics;
using App.Metrics.Reporting.Wavefront.Builder;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Prometheus;
using System.Collections.Immutable;
using System.Net;
using System.Reflection.Emit;
using Tap.Dotnet.Weather.Api.Interfaces;
using Tap.Dotnet.Weather.Domain;
using Wavefront.SDK.CSharp.Common;
using Wavefront.SDK.CSharp.Common.Application;
using WeatherBit.Domain;
using WeatherBit.Domain.Interfaces;

namespace Tap.Dotnet.Weather.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ForecastController : ControllerBase
    {
        private readonly IWeatherBitService weatherBitService;
        private readonly IWavefrontSender wavefrontSender;
        private readonly IWeatherDataService weatherDataService;
        private readonly ILogger<ForecastController> logger;

        private static readonly Counter TempsBelowZero = Metrics
            .CreateCounter("TempsBelowZero", "Number of temperatures below zero.",
                new CounterConfiguration { SuppressInitialValue = true });
        private static readonly Counter TempsAbove100 = Metrics
            .CreateCounter("TempsAboveOneHundred", "Number of temperatures above one hundred.",
                new CounterConfiguration { SuppressInitialValue = true });

        private static readonly string[] Summaries = new[]
{
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public ForecastController(
            IWeatherBitService weatherBitService, 
            IWavefrontSender wavefrontSender, 
            IWeatherDataService weatherDataService,
            ILogger<ForecastController> logger)
        {
            this.weatherBitService = weatherBitService;
            this.wavefrontSender = wavefrontSender;
            this.weatherDataService = weatherDataService;
            this.logger = logger;
        }

        [HttpGet]
        [Route("{zipCode}")]
        public WeatherInfo Get(string zipCode)
        {
            var weatherInfo = new WeatherInfo();

            var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    return true;
                };

                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.BaseAddress = new Uri(this.weatherBitService.Url);

                    var key = this.weatherBitService.Key;

                    var response = httpClient.GetAsync($"forecast/daily?postal_code={zipCode}&key={key}").Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;

                        var serializerSettings = new JsonSerializerSettings();
                        serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

                        var weatherBitInfo = JsonConvert.DeserializeObject<WeatherBitInfo>(content, serializerSettings);

                        weatherInfo.CityName = weatherBitInfo.city_name;
                        weatherInfo.StateCode = weatherBitInfo.state_code;
                        weatherInfo.CountryCode = weatherBitInfo.country_code;
                        weatherInfo.Latitude = weatherBitInfo.lat;
                        weatherInfo.Longitude = weatherBitInfo.lon;
                        weatherInfo.TimeZone = weatherBitInfo.timezone;

                        foreach (var weatherBitForecast in weatherBitInfo.data)
                        {
                            var weatherForecast = new WeatherForecast();
                            weatherForecast.Date = Convert.ToDateTime(weatherBitForecast.datetime);
                            weatherForecast.TemperatureC = Convert.ToSingle(weatherBitForecast.temp);
                            weatherForecast.Description = weatherBitForecast.weather.description;

                            weatherInfo.Forecast.Add(weatherForecast);
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.TooManyRequests) // if free limits are exceeded, return random
                    {
                        weatherInfo = GetRandom();
                    }
                }
            }

            var min = Convert.ToDouble(weatherInfo.Forecast.Min(t => t.TemperatureC));
            var max = Convert.ToDouble(weatherInfo.Forecast.Max(t => t.TemperatureC));
            var tags = new Dictionary<string, string>();

            tags.Add("DeploymentType", "Environment");

            // save as storage in wavefront
            this.wavefrontSender.SendMetric("MinimumRandomForecast", min,
                DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow), "tap-dotnet-weather-api", tags);
            this.wavefrontSender.SendMetric("MaximumRandomForecast", max,
                DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow), "tap-dotnet-weather-api", tags);

            // report metrics
            var applicationTags = new ApplicationTags.Builder("tap-dotnet-weather-api", "forecast-controller").Build();

            var metricsBuilder = new MetricsBuilder();

            metricsBuilder.Report.ToWavefront(
              options =>
              {
                  options.WavefrontSender = this.wavefrontSender;
                  options.Source = "tap-dotnet-weather-api"; // optional
                  options.WavefrontHistogram.ReportMinuteDistribution = true; // optional
                  options.ApplicationTags = applicationTags;
              });

            var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            if(this.Request.Headers.ContainsKey("X-TraceId") && this.Request.Headers.ContainsKey("X-SpanId"))
            {
                var traceId = this.Request.Headers["X-TraceId"][0];
                var spanId = this.Request.Headers["X-SpanId"][0];

                this.wavefrontSender.SendSpan(
                    "GetWeatherForecast", start, end, "WeatherApi", new Guid(traceId), Guid.NewGuid(),
                    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                    ImmutableList.Create(
                        new KeyValuePair<string, string>("application", "tap-dotnet-weather-api"),
                        new KeyValuePair<string, string>("service", "GetWeatherForecast"),
                        new KeyValuePair<string, string>("zipcode", zipCode),
                        new KeyValuePair<string, string>("http.method", "GET")), null);
            }

            return weatherInfo;
        }

        [HttpGet]
        [Route("random")]
        public WeatherInfo GetRandom()
        {
            var weatherInfo = new WeatherInfo();

            var start = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);
            Thread.Sleep(100);
            var end = DateTimeUtils.UnixTimeMilliseconds(DateTime.UtcNow);

            if (this.Request.Headers.ContainsKey("X-TraceId") && this.Request.Headers.ContainsKey("X-SpanId"))
            {
                var traceId = this.Request.Headers["X-TraceId"][0];
                var spanId = this.Request.Headers["X-SpanId"][0];

                this.wavefrontSender.SendSpan(
                    "GetWeatherForecast", start, end, "WeatherApi", new Guid(traceId), Guid.NewGuid(),
                    ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                    ImmutableList.Create(
                        new KeyValuePair<string, string>("application", "tap-dotnet-weather-api"),
                        new KeyValuePair<string, string>("service", "GetWeatherForecast"),
                        new KeyValuePair<string, string>("zipcode", "random"),
                        new KeyValuePair<string, string>("http.method", "GET")), null);
            }

            weatherInfo.CityName = "Palo Alto";
            weatherInfo.StateCode = "CA";
            weatherInfo.CountryCode = "US";

            var forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Description = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            var belowZero = forecast.Count(f => f.TemperatureF < 0);
            var above100 = forecast.Count(f => f.TemperatureF > 100);

            TempsBelowZero.Inc(belowZero);
            TempsAbove100.Inc(above100);

            weatherInfo.Forecast = forecast;

            return weatherInfo;
        }
    }
}
