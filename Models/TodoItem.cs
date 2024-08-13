namespace TodoApi.Models
{
    public class Film
    {
        public long Id { get; set; }
        public string? name { get; set; } // Nullable özellik
        public string? genre { get; set; }
        public int year { get; set; }
        public string? director { get; set; }
        public double imdbRating { get; set; }
    }
}
