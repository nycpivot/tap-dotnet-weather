using System.ComponentModel.DataAnnotations;

namespace Tap.Dotnet.Domain
{
    public class Favorite
    {
        [Key]
        public string ZipCode { get; set; } = String.Empty;
    }
}
