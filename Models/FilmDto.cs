// Models/FilmDto.cs
using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class FilmDto
    {
        [Required(ErrorMessage = "Film adı zorunludur.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Film türü zorunludur.")]
        public string? Genre { get; set; }

        [Required(ErrorMessage = "Yönetmen adı zorunludur.")]
        public string? Director { get; set; }

        [Range(1900, 2100, ErrorMessage = "Yıl 1900 ile 2100 arasında olmalıdır.")]
        public int ReleaseYear { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "IMDb puanı 0 ile 10 arasında olmalıdır.")]
        public float IMDbRating { get; set; }
    }
}
