using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Diagnostics;
using System.Reflection;
using Tap.Dotnet.Common.Interfaces;
using Tap.Dotnet.Web.Application.Interfaces;
using Tap.Dotnet.Web.Application.Models;
using Tap.Dotnet.Web.Mvc.Models;

namespace Tap.Dotnet.Web.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDatabase cache;
        private readonly IWeatherApplication weatherApplication;
        private readonly IApiHelper apiHelper;
        private readonly ILogger<HomeController> logger;

        public HomeController(
            IDatabase cache,
            IWeatherApplication weatherApplication,
            IApiHelper apiHelper, ILogger<HomeController> logger)
        {
            this.cache = cache;
            this.weatherApplication = weatherApplication;
            this.apiHelper = apiHelper;
            this.logger = logger;
        }

        public IActionResult Index(HomeViewModel model)
        {
            var homeViewModel = new HomeViewModel(); // this.weatherApplication.GetForecast(model.WeatherInfo.ZipCode);

            homeViewModel.Recents = Cache(model.WeatherInfo.ZipCode);

            //var recentZips = this.cache.StringGet(new RedisKey("Recent"));
            //var zipList = JsonConvert.DeserializeObject<List<string>>(recentZips);
            //if(!zipList.Any(z => z == model.WeatherInfo.ZipCode))
            //{
            //    zipList.Add(model.WeatherInfo.ZipCode);
            //    recentZips = JsonConvert.SerializeObject(zipList);

            //    this.cache.SetAdd(new RedisKey("Recent"), recentZips);
            //}

            return View(homeViewModel);
        }

        [HttpPost]
        public ActionResult Search(HomeViewModel model)
        {
            var homeViewModel = this.weatherApplication.GetForecast(model.WeatherInfo.ZipCode);

            homeViewModel.Recents = Cache(model.WeatherInfo.ZipCode);

            return View("Index", homeViewModel);
        }

        [HttpPost]
        public ActionResult Save(HomeViewModel model)
        {
            var homeViewModel = this.weatherApplication.SaveFavorite(model.WeatherInfo.ZipCode);

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
