using Tap.Dotnet.Weather.Application.Models;

namespace Tap.Dotnet.Weather.Application.Interfaces
{
    public interface IWeatherApplication
    {
        HomeViewModel GetForecast(string zipCode, Guid traceId, Guid spanId);
        HomeViewModel GetRandom(Guid traceId, Guid spanId);
        HomeViewModel SaveFavorite(string zipCode, Guid traceId, Guid spanId);
    }
}
