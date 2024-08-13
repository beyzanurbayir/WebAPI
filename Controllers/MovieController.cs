using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controllers;

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
        new Film { Id = 10, name = "Pulp Fiction", genre = "Crime, Drama", year = 1994, director = "Quentin Tarantino", imdbRating = 8.9 }
    };

    private readonly ILogger<FilmController> _logger;

    public FilmController(ILogger<FilmController> logger)
    {
        _logger = logger;
    }

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

    [HttpGet("all")]
    public IActionResult GetAllMovies()
    {
        return Ok(Films);
    }
}
