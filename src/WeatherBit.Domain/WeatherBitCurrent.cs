namespace WeatherBit.Domain
{
    public class WeatherBitCurrent
    {
        public string city_name { get; set; } = String.Empty;
        public string state_code { get; set; } = String.Empty;
        public string country_code { get; set; } = String.Empty;
        public string lat { get; set; } = String.Empty;
        public string lon { get; set; } = String.Empty;
        public string timezone { get; set; } = String.Empty;
        public string temp { get; set; } = String.Empty;
        public string clouds { get; set; } = String.Empty;
        public string datetime { get; set; } = String.Empty;
        public string sunrise { get; set; } = String.Empty;
        public string sunset { get; set; } = String.Empty;
        public string precip { get; set; } = String.Empty;
        public string snow { get; set; } = String.Empty;

        public WeatherBitDescription weather { get; set; } = new WeatherBitDescription();
    }
}
