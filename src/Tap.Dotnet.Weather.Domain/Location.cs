using System.ComponentModel.DataAnnotations;

namespace Tap.Dotnet.Domain
{
    public class Location
    {
        [Key]
        public string ZipCode { get; set; } = String.Empty;
        public string Latitude { get; set; } = String.Empty;
        public string Longitude { get; set; } = String.Empty;
        public string CityName { get; set; } = String.Empty;
        public string StateCode { get; set; } = String.Empty;
        public string StateName { get; set; } = String.Empty;
        public int Population { get; set; }
    }
}
