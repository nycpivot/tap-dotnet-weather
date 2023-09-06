using Tap.Dotnet.Weather.Application.Interfaces;

namespace Tap.Dotnet.Weather.Application.Models
{
    public class WeatherApi : IWeatherApi
    {
        public string Url { get; set; } = String.Empty;
    }
}
