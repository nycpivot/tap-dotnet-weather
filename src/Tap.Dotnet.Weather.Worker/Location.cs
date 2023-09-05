using System.ComponentModel.DataAnnotations;

namespace Tap.Dotnet.Domain
{
    public class Location
    {
        [Key]
        public string ZipCode { get; set; } = String.Empty;
        public Single Latitude { get; set; }
        public Single Longitude { get; set; }
        public string CityName { get; set; } = String.Empty;
        public string StateCode { get; set; } = String.Empty;
        public string StateName { get; set; } = String.Empty;
        public int Population { get; set; }
    }
}
