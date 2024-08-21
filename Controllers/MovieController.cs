using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using TodoApi.Models; 

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilmController : ControllerBase  //Repository'i kullanarak veri erişim işlemlerini gerçekleştirir
    {
        //private readonly MovieDbContext _context;
        private readonly IFilmRepository _filmRepository;
        private readonly RedisCacheService _cacheService;
        private readonly ILogger<FilmController> _logger;

        // public FilmController(MovieDbContext context, ILogger<FilmController> logger)
        // {
        //     _context = context;
        //     _logger = logger;
        // }
        public FilmController(IFilmRepository filmRepository, RedisCacheService cacheService, ILogger<FilmController> logger)
    {
        _filmRepository = filmRepository;
        _cacheService = cacheService;
        _logger = logger;
    }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
             string cacheKey = $"film_{id}";
            //var film = _context.Films.FirstOrDefault(f => f.Id == id);
            //var film = _filmRepository.GetById(id);
            var film = _cacheService.Get<Film>(cacheKey);
            if (film == null)
            {
                film = _filmRepository.GetById(id);
                if (film == null)
                {
                    return NotFound(new { message = "Film bulunamadı." });
                }
                _cacheService.Set(cacheKey, film);
            }

            return Ok(film);
        }

        // Tek bir film arama
  
            [HttpGet("name/{name}")]
            public ActionResult<Film> GetByName(string name)
            {
                string cacheKey = $"film_{name.ToLower()}";
                var film = _cacheService.Get<Film>(cacheKey);

                if (film == null)
                {
                    film = _filmRepository.GetAll()
                        .FirstOrDefault(f => f.Title != null && f.Title.ToLower() == name.ToLower());

                    if (film == null)
                    {
                        return NotFound(new { message = "Film bulunamadı." });
                    }
                    _cacheService.Set(cacheKey, film);
                }

                return Ok(film);
                    }


        // Tüm film isimlerini listeleme
        [HttpGet("names")]
        public IActionResult GetAllFilmNames()
        {
            //var filmNames = _context.Films.Select(f => f.Title).ToList();
            string cacheKey = "film_names";
            var filmNames = _filmRepository.GetAll()
                .Select(f => f.Title)
                .ToList();
            if (filmNames == null)
                {
                    filmNames = _filmRepository.GetAll()
                        .Select(f => f.Title)
                        .ToList();

                    if (filmNames == null || !filmNames.Any())
                    {
                        return NotFound("Hiç film bulunamadı.");
                    }
                    _cacheService.Set(cacheKey, filmNames);
                }

    return Ok(filmNames);
        }

        // Film ekleme
       [HttpPost]
        public IActionResult Post([FromBody] FilmDto filmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // DTO'dan Film entity'sine dönüşüm
            var film = new Film
            {
                Title = filmDto.Title,
                Genre = filmDto.Genre,
                Director = filmDto.Director,
                ReleaseYear = filmDto.ReleaseYear,
                IMDbRating = filmDto.IMDbRating
            };

            _filmRepository.Add(film);

             // Redis'i güncelle
            string cacheKey = $"film_{film.Title.ToLower()}";
            _cacheService.Set(cacheKey, film);

            // Film eklendikten sonra GetById metodunu çağırarak film detaylarını döndür
            return CreatedAtAction(nameof(GetById), new { id = film.Id }, film);
        }

        [HttpDelete("name/{name}")]
        public IActionResult DeleteByName(string name)
        {
            var film = _filmRepository.GetAll()
                .FirstOrDefault(f => f.Title != null && f.Title.ToLower() == name.ToLower());

            if (film == null)
            {
                return NotFound(new { message = "Film bulunamadı." });
            }

            _filmRepository.Delete(film.Id); // Film'in ID'sini kullanarak silme işlemi gerçekleştiriyoruz.


            // Redis'ten kaldır
            string cacheKey = $"film_{name.ToLower()}";
            _cacheService.Remove(cacheKey);

            return NoContent(); // Başarılı silme işlemi için HTTP 204 No Content döndürür.
        }

    }
}
