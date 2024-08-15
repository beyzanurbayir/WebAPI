using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using System.Linq;
using System.Collections.Generic;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilmController : ControllerBase
    {
        private static readonly List<Film> Films = new List<Film>
        {
            new Film { Id = 1, name = "The Shawshank Redemption", genre = "Drama", year = 1994, director = "Frank Darabont", imdbRating = 9.3 },
            new Film { Id = 2, name = "The Godfather", genre = "Crime, Drama", year = 1972, director = "Francis Ford Coppola", imdbRating = 9.2 },
            new Film { Id = 3, name = "The Dark Knight", genre = "Action, Crime, Drama", year = 2008, director = "Christopher Nolan", imdbRating = 9.0 },
            new Film { Id = 4, name = "Forrest Gump", genre = "Drama, Romance", year = 1994, director = "Robert Zemeckis", imdbRating = 8.8 },
            new Film { Id = 5, name = "Inception", genre = "Action, Adventure, Sci-Fi", year = 2010, director = "Christopher Nolan", imdbRating = 8.8 },
            new Film { Id = 6, name = "Interstellar", genre = "Adventure, Drama, Sci-Fi", year = 2014, director = "Christopher Nolan", imdbRating = 8.6 },
            new Film { Id = 7, name = "The Matrix", genre = "Action, Sci-Fi", year = 1999, director = "Lana Wachowski, Lilly Wachowski", imdbRating = 8.7 },
            new Film { Id = 8, name = "The Lord of the Rings: The Return of the King", genre = "Action, Adventure, Drama", year = 2003, director = "Peter Jackson", imdbRating = 8.9 },
            new Film { Id = 9, name = "Fight Club", genre = "Drama", year = 1999, director = "David Fincher", imdbRating = 8.8 },
            new Film { Id = 10, name = "Pulp Fiction", genre = "Crime, Drama", year = 1994, director = "Quentin Tarantino", imdbRating = 8.9 },
            new Film { Id = 11, name = "The Prestige", genre = "Drama, Mystery, Sci-Fi", year = 2006, director = "Christopher Nolan", imdbRating = 8.5 },
            new Film { Id = 12, name = "Se7en", genre = "Crime, Drama, Mystery", year = 1995, director = "David Fincher", imdbRating = 8.6 },
            new Film { Id = 13, name = "The Departed", genre = "Crime, Drama, Thriller", year = 2006, director = "Martin Scorsese", imdbRating = 8.5 },
            new Film { Id = 14, name = "Gladiator", genre = "Action, Adventure, Drama", year = 2000, director = "Ridley Scott", imdbRating = 8.5 },
            new Film { Id = 15, name = "Parasite", genre = "Comedy, Drama, Thriller", year = 2019, director = "Bong Joon-ho", imdbRating = 8.6 },
            new Film { Id = 16, name = "The Lion King", genre = "Animation, Adventure, Drama", year = 1994, director = "Roger Allers, Rob Minkoff", imdbRating = 8.5 },
            new Film { Id = 17, name = "The Social Network", genre = "Biography, Drama", year = 2010, director = "David Fincher", imdbRating = 7.7 }

        };

        private readonly ILogger<FilmController> _logger;

        public FilmController(ILogger<FilmController> logger)
        {
            _logger = logger;
        }

        // Tek bir film arama
        [HttpGet("name/{name}")]
        public ActionResult<Film> GetByName(string name)
        {
            var film = Films.FirstOrDefault(f => f.name != null && f.name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (film == null)
            {
                return NotFound();
            }
            return Ok(film);
        }


        // Tüm film isimlerini listeleme
        [HttpGet("names")]
        public IActionResult GetAllFilmNames()
        {
            var filmNames = Films.Select(f => f.name).ToList();

            if (filmNames == null || !filmNames.Any())
            {
                return NotFound("Hiç film bulunamadı.");
            }

            return Ok(filmNames);
        }
 // Film ekleme
      [HttpPost]
public IActionResult Post([FromBody] Film film)
{
    if (film == null)
    {
        return BadRequest(new { message = "Film verisi boş." });
    }

    // "year" değişkeni için 4 basamak kontrolü
    if (film.year.ToString().Length != 4)
    {
        return BadRequest(new { message = "Yıl 4 basamaktan oluşmalıdır." });
    }

    // "imdbRating" değişkeni için 0.0 ile 10.0 arasında olma kontrolü
    if (film.imdbRating < 0.0 || film.imdbRating > 10.0)
    {
        return BadRequest(new { message = "IMDb puanı 0 ile 10 arasında olmalıdır." });
    }

    // Film Id'sini dinamik olarak oluşturun
    film.Id = Films.Count > 0 ? Films.Max(f => f.Id) + 1 : 1;

    Films.Add(film);
    return CreatedAtAction(nameof(GetByName), new { name = film.name }, film);
}



    }

}
