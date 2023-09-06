using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Immutable;
using System.Diagnostics;
using Tap.Dotnet.Weather.Application.Interfaces;
using Tap.Dotnet.Weather.Application.Models;
using Tap.Dotnet.Weather.Web.Models;
using Wavefront.SDK.CSharp.Common;
using Wavefront.SDK.CSharp.DirectIngestion;

namespace Tap.Dotnet.Weather.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDatabase cache;
        private readonly IWeatherApplication weatherApplication;
        private readonly WavefrontDirectIngestionClient wavefrontSender;
        private readonly ILogger<HomeController> logger;

        public HomeController(
            IDatabase cache, IWeatherApplication weatherApplication,
            WavefrontDirectIngestionClient wavefrontSender, ILogger<HomeController> logger)
        {
            this.cache = cache;
            this.weatherApplication = weatherApplication;
            this.wavefrontSender = wavefrontSender;
            this.logger = logger;
        }

        public IActionResult Index(HomeViewModel model)
        {
            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            this.wavefrontSender.SendSpan(
                "Index", 0, 1, "tap-dotnet-core-web-mvc-env", traceId, Guid.NewGuid(),
                ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                ImmutableList.Create(
                    new KeyValuePair<string, string>("application", "tap-dotnet-core-web-mvc-env"), // "tap-dotnet-weather-web"),
                    new KeyValuePair<string, string>("service", "Index"),
                    new KeyValuePair<string, string>("http.method", "GET")), null);

            var homeViewModel = this.weatherApplication.GetForecast(model.WeatherInfo.ZipCode, traceId, spanId);

            homeViewModel.Recents = Cache(model.WeatherInfo.ZipCode);

            return View(homeViewModel);
        }

        [HttpPost]
        public ActionResult Search(HomeViewModel model)
        {
            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            this.wavefrontSender.SendSpan(
                "Search", 0, 1, "tap-dotnet-core-web-mvc-env", traceId, Guid.NewGuid(),
                ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                ImmutableList.Create(
                    new KeyValuePair<string, string>("application", "tap-dotnet-core-web-mvc-env"),
                    new KeyValuePair<string, string>("service", "Search"),
                    new KeyValuePair<string, string>("http.method", "GET")), null);

            var homeViewModel = this.weatherApplication.GetForecast(model.WeatherInfo.ZipCode, traceId, spanId);

            homeViewModel.Recents = Cache(model.WeatherInfo.ZipCode);

            return View("Index", homeViewModel);
        }

        [HttpPost]
        public ActionResult Save(HomeViewModel model)
        {
            var traceId = Guid.NewGuid();
            var spanId = Guid.NewGuid();

            this.wavefrontSender.SendSpan(
                "Save", 0, 1, "tap-dotnet-core-web-mvc-env", traceId, Guid.NewGuid(),
                ImmutableList.Create(new Guid("82dd7b10-3d65-4a03-9226-24ff106b5041")), null,
                ImmutableList.Create(
                    new KeyValuePair<string, string>("application", "tap-dotnet-core-web-mvc-env"),
                    new KeyValuePair<string, string>("service", "Save"),
                    new KeyValuePair<string, string>("http.method", "POST")), null);

            var homeViewModel = this.weatherApplication.SaveFavorite(model.WeatherInfo.ZipCode, traceId, spanId);

            homeViewModel.Recents = Cache(model.WeatherInfo.ZipCode);

            return View("Index", homeViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IList<RecentViewModel> Cache(string zipCode)
        {
            var recents = new List<RecentViewModel>();

            //this.cache.KeyDelete(new RedisKey("Recent"));

            var recent = this.cache.StringGet(new RedisKey("Recent"));

            if (!recent.HasValue)
            {
                var zipList = new List<string>() { zipCode };

                var recentList = JsonConvert.SerializeObject(zipList);

                this.cache.StringSet(new RedisKey("Recent"), new RedisValue(recentList));

                recents.Add(new RecentViewModel { ZipCode = zipCode });
            }
            else
            {
                var zipList = JsonConvert.DeserializeObject<IList<string>>(recent);

                if (!zipList.Any(z => z == zipCode))
                {
                    zipList.Add(zipCode);
                }

                var recentList = JsonConvert.SerializeObject(zipList);

                this.cache.StringSet(new RedisKey("Recent"), new RedisValue(recentList));

                foreach(var zip in zipList)
                {
                    recents.Add(new RecentViewModel() { ZipCode = zip });
                }
            }

            return recents;
        }
    }
}
