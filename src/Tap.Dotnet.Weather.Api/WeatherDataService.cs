using Tap.Dotnet.Weather.Api.Interfaces;

namespace Tap.Dotnet.Weather.Api
{
    public class WeatherDataService : IWeatherDataService
    {
        public string Url { get; set; } = String.Empty;
    }
}
