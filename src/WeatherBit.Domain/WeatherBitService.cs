using WeatherBit.Domain.Interfaces;

namespace WeatherBit.Domain
{
    public class WeatherBitService : IWeatherBitService
    {
        public string Url { get; set; }
        public string Key { get; set; }
    }
}
