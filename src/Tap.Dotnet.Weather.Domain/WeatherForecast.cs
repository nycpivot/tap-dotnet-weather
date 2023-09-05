namespace Tap.Dotnet.Domain
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public Single TemperatureC { get; set; }

        public Single TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Description { get; set; }
    }
}