using Tap.Dotnet.Weather.Application.Models;

namespace Tap.Dotnet.Weather.Application.Interfaces
{
    public interface IWeatherApplication
    {
        HomeViewModel GetForecast(string zipCode);
        HomeViewModel SaveFavorite(string zipCode);
    }
}
