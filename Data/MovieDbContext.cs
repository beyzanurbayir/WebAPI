using Microsoft.EntityFrameworkCore;

public class MovieDbContext : DbContext
{public MovieDbContext(DbContextOptions<MovieDbContext> options)
    : base(options)
{ }
    public DbSet<Film> Films { get; set; } // 'Movies' yerine 'Films' kullanılıyor

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=MovieDatabase;User Id=sa;Password=Password1;TrustServerCertificate=True;");
    }
}

public class Film
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Director { get; set; }
    public string? Genre { get; set; }
    public int ReleaseYear { get; set; }
    public float IMDbRating { get; set; }
}
