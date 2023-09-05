using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Tap.Dotnet.Common.Interfaces;
using Tap.Dotnet.Web.Application.Interfaces;
using Tap.Dotnet.Web.Mvc.Models;

namespace Tap.Dotnet.Web.Mvc.Controllers
{
    public class EnvironmentController : Controller
    {
        private readonly IWeatherApplication weatherApplication;
        private readonly IApiHelper apiHelper;
        private readonly ILogger<HomeController> _logger;

        public EnvironmentController(
            IWeatherApplication weatherApplication,
            IApiHelper apiHelper,
            ILogger<HomeController> logger)
        {
            this.weatherApplication = weatherApplication;
            this.apiHelper = apiHelper;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // list environment variables
            try
            {
                var variables = new List<EnvironmentVariable>();

                try
                {
                    var environment = Environment.GetEnvironmentVariables();
                    foreach (DictionaryEntry variable in environment)
                    {
                        var ev = new EnvironmentVariable()
                        {
                            Key = variable.Key.ToString() ?? String.Empty,
                            Value = variable.Value?.ToString() ?? String.Empty
                        };

                        variables.Add(ev);
                    }
                }
                catch
                {
                    // send errors somewhere
                }

                ViewBag.Variables = variables.OrderBy(e => e.Key).ToList();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Environment", ex.StackTrace ?? ex.Message);
            }

            return View();
        }
    }
}
