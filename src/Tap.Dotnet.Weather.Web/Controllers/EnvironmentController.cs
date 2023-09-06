using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Tap.Dotnet.Weather.Application.Interfaces;
using Tap.Dotnet.Weather.Web.Models;

namespace Tap.Dotnet.Weather.Web.Controllers
{
    public class EnvironmentController : Controller
    {
        private readonly IWeatherApplication weatherApplication;
        private readonly ILogger<HomeController> _logger;

        public EnvironmentController(
            IWeatherApplication weatherApplication,
            ILogger<HomeController> logger)
        {
            this.weatherApplication = weatherApplication;
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
