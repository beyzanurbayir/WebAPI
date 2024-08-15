using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class Film
    {public long Id { get; set; }

    [Required]
    public string? name { get; set; } // Nullable özellik

    [Required]
    public string? genre { get; set; }

    [Range(1900, 2099, ErrorMessage = "Yıl 4 basamaktan oluşmalıdır.")]
    public int year { get; set; }

    [Range(0.0, 10.0, ErrorMessage = "IMDb puanı 0 ile 10 arasında olmalıdır.")]
    public double imdbRating { get; set; }

    public string? director { get; set; }
    }
}
