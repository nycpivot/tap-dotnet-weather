using System.ComponentModel.DataAnnotations;

namespace Tap.Dotnet.Weather.Domain
{
    public class Favorite
    {
        [Key]
        public string ZipCode { get; set; } = String.Empty;
    }
}
