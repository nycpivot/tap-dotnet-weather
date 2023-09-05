namespace WeatherBit.Domain
{
    public class WeatherBitForecast
    {
        public string temp { get; set; } = String.Empty;
        public string clouds { get; set; } = String.Empty;
        public string datetime { get; set; } = String.Empty;
        public string precip { get; set; } = String.Empty;
        public string snow { get; set; } = String.Empty;
        public string snow_depth { get; set; } = String.Empty;
        public string sunrise_ts { get; set; } = String.Empty;
        public string sunset_ts { get; set; } = String.Empty;

        public WeatherBitDescription weather { get; set; } = new WeatherBitDescription();
    }
}
