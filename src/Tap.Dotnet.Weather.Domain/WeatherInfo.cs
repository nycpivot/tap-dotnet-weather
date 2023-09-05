namespace Tap.Dotnet.Domain
{
    public class WeatherInfo
    {
        public string CityName { get; set; } = String.Empty;
        public string StateCode { get; set; } = String.Empty;
        public string CountryCode { get; set; } = String.Empty;
        public string Latitude { get; set; } = String.Empty;
        public string Longitude { get; set; } = String.Empty;
        public string TimeZone { get; set; } = String.Empty;

        public IList<WeatherForecast> Forecast { get; set; } = new List<WeatherForecast>();
    }
}
