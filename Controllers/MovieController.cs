using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilmController : ControllerBase
    {
        private readonly MovieDbContext _context;
        private readonly ILogger<FilmController> _logger;

        public FilmController(MovieDbContext context, ILogger<FilmController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var film = _context.Films.FirstOrDefault(f => f.Id == id);
            if (film == null)
            {
                return NotFound(new { message = "Film bulunamadı." });
            }

            return Ok(film);
        }

        // Tek bir film arama
  
            [HttpGet("name/{name}")]
            public ActionResult<Film> GetByName(string name)
            {
                try
                {
                    var film = _context.Films
                        .FirstOrDefault(f => f.Title != null && f.Title.ToLower() == name.ToLower());

                    if (film == null)
                    {
                        return NotFound(new { message = "Film bulunamadı." });
                    }

                    return Ok(film);
                }
                catch (Exception ex)
                {
                    // Hata durumunda 500 kodu ve hata mesajını döndür
                    return StatusCode(500, $"Sunucu hatası: {ex.Message}");
                }
            }


        // Tüm film isimlerini listeleme
        [HttpGet("names")]
        public IActionResult GetAllFilmNames()
        {
            var filmNames = _context.Films.Select(f => f.Title).ToList();

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
                if (film.ReleaseYear.ToString().Length != 4)
                {
                    return BadRequest(new { message = "Yıl 4 basamaktan oluşmalıdır." });
                }

                // "imdbRating" değişkeni için 0.0 ile 10.0 arasında olma kontrolü
                if (film.IMDbRating < 0.0 || film.IMDbRating > 10.0)
                {
                    return BadRequest(new { message = "IMDb puanı 0 ile 10 arasında olmalıdır." });
                }

                _context.Films.Add(film); 
                _context.SaveChanges();

                // Film eklendikten sonra GetById metodunu çağırarak film detaylarını döndür
                return CreatedAtAction(nameof(GetById), new { id = film.Id }, film);
            }

    }
}
