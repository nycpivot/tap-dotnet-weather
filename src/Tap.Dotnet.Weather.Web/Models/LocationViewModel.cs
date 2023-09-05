namespace Tap.Dotnet.Web.Mvc.Models
{
    public class LocationViewModel
    {
        public string ZipCode { get; set; } = String.Empty;
        public string Latitude { get; set; } = String.Empty;
        public string Longitude { get; set; } = String.Empty;
        public string CityName { get; set; } = String.Empty;
        public string StateCode { get; set; } = String.Empty;
        public string StateName { get; set; } = String.Empty;
        public int Population { get; set; }
    }
}
