namespace WeatherBit.Domain
{
    public class WeatherBitInfo
    {
        public string city_name { get; set; } = String.Empty;
        public string state_code { get; set; } = String.Empty;
        public string country_code { get; set; } = String.Empty;
        public string lat { get; set; } = String.Empty;
        public string lon { get; set; } = String.Empty;
        public string timezone { get; set; } = String.Empty;

        
        public IList<WeatherBitForecast> data { get; set; } = new List<WeatherBitForecast>();
    }
}
