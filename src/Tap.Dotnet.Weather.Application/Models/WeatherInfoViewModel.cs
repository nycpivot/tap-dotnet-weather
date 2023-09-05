namespace Tap.Dotnet.Web.Application.Models
{
    public class WeatherInfoViewModel
    {
        public string ZipCode { get; set; } = "95131"; // Palo Alto
        public string CityName { get; set; } = String.Empty;
        public string StateCode { get; set; } = String.Empty;
        public string CountryCode { get; set; } = String.Empty;
        public string Latitude { get; set; } = String.Empty;
        public string Longitude { get; set; } = String.Empty;
        public string TimeZone { get; set; } = String.Empty;

        public IList<WeatherForecastViewModel> Forecast { get; set; } = new List<WeatherForecastViewModel>();
    }
}
