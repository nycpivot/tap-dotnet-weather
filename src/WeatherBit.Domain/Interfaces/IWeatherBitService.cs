namespace WeatherBit.Domain.Interfaces
{
    public interface IWeatherBitService
    {
        string Url { get; set; }
        string Key { get; set; }
    }
}
