//Controllers/MovieController.cs

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
        private readonly IFilmRepository _filmRepository;
        private readonly RedisCacheService _cacheService;
        private readonly ILogger<FilmController> _logger;

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
            var filmDto = _cacheService.Get<FilmDto>(cacheKey);

            if (filmDto == null)
            {
                var film = _filmRepository.GetById(id);  
                if (film == null)
                {
                    return NotFound(new { message = "Film bulunamadı." });
                }
                filmDto = new FilmDto
                {
                    id = film.Id,  
                    Title = film.Title,
                    Genre = film.Genre,
                    Director = film.Director,
                    ReleaseYear = film.ReleaseYear,
                    IMDbRating = film.IMDbRating
                };

                _cacheService.Set(cacheKey, filmDto);
            }

            return Ok(filmDto);
        }


        // Tek bir film arama
        [HttpGet("name/{name}")]
        public ActionResult<FilmDto> GetByName(string name)
        {
            string cacheKey = $"film_{name.ToLower()}";
            var filmDto = _cacheService.Get<FilmDto>(cacheKey);

            if (filmDto == null)
            {
                var film = _filmRepository.GetAll()
                    .FirstOrDefault(f => f.Title != null && f.Title.ToLower() == name.ToLower());

                if (film == null)
                {
                    return NotFound(new { message = "Film bulunamadı." });
                }

                filmDto = new FilmDto
                {
                    id = film.Id,
                    Title = film.Title,
                    Genre = film.Genre,
                    Director = film.Director,
                    ReleaseYear = film.ReleaseYear,
                    IMDbRating = film.IMDbRating
                };

                _cacheService.Set(cacheKey, filmDto);
            }

            return Ok(filmDto);
        }



        [HttpGet("names")]
        public IActionResult GetAllFilmNames()
        {
            string cacheKey = "film_names";
            var filmNames = _cacheService.Get<List<string>>(cacheKey);

            // Veritabanındaki tüm film isimlerini al
            var allFilmNames = _filmRepository.GetAll()
                .Select(f => f.Title.ToLower())
                .ToList();

            if (filmNames != null)
            {
                // Silinen film isimlerini kontrol et ve kaldır
                var outdatedFilmNames = filmNames
                    .Where(name => !allFilmNames.Contains(name.ToLower()))
                    .ToList();

                if (outdatedFilmNames.Any())
                {
                    foreach (var outdatedName in outdatedFilmNames)
                    {
                        filmNames.Remove(outdatedName);
                    }
                    // Güncellenmiş listeyi Redis'e kaydet
                    _cacheService.Set(cacheKey, filmNames);
                }
            }

            // POST işlemi yapılmış mı kontrol et
            var postFilmName = _cacheService.Get<string>("post_film_name");

            if (postFilmName != null)
            {
                // POST işlemi yapılmışsa, listeye eklenen filmi de ekle
                if (filmNames == null)
                {
                    // Eğer film isimleri daha önce yüklenmemişse, tüm isimleri yeniden yükle
                    filmNames = _filmRepository.GetAll()
                        .Select(f => f.Title)
                        .ToList();
                }

                // Eklenen filmi listeye ekle
                if (!filmNames.Contains(postFilmName))
                {
                    filmNames.Add(postFilmName);
                    // Güncellenmiş listeyi Redis'e kaydet
                    _cacheService.Set(cacheKey, filmNames);
                }

                // POST işlemi sonrası flag'i sıfırla
                _cacheService.Remove("post_film_name");
            }
            else
            {
                // POST işlemi yapılmamışsa ve Redis'te film isimleri yoksa, veritabanından yükle
                if (filmNames == null)
                {
                    // Veritabanındaki tüm film DTO'larını al
                    var allFilmDtos = _filmRepository.GetAll()
                        .Select(f => new FilmDto
                        {
                            id = f.Id,
                            Title = f.Title,
                            Genre = f.Genre,
                            Director = f.Director,
                            ReleaseYear = f.ReleaseYear,
                            IMDbRating = f.IMDbRating
                        })
                        .ToList();

                    // Listeyi oluştur
                    filmNames = allFilmDtos
                        .Select(dto => dto.Title)
                        .ToList();

                    if (filmNames == null || !filmNames.Any())
                    {
                        return NotFound("Hiç film bulunamadı.");
                    }

                    // Listeyi Redis'e kaydet
                    _cacheService.Set(cacheKey, filmNames);
                }
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

            // Film veritabanına ekle
            _filmRepository.Add(film);

            // Redis'i güncelle
            string cacheKey = $"film_{film.Title.ToLower()}";
            var filmDtoToCache = new FilmDto
            {
                id = filmDto.id,
                Title = filmDto.Title,
                Genre = filmDto.Genre,
                Director = filmDto.Director,
                ReleaseYear = filmDto.ReleaseYear,
                IMDbRating = filmDto.IMDbRating
            };
            _cacheService.Set(cacheKey, filmDtoToCache);

            // Film isimlerini listeleyici cache'i güncelle
            string filmNamesCacheKey = "film_names";
            var filmNames = _cacheService.Get<List<string>>(filmNamesCacheKey);

            if (filmNames == null)
            {
                // Eğer film isimleri daha önce yüklenmemişse, tüm isimleri yeniden yükle
                filmNames = _filmRepository.GetAll()
                    .Select(f => f.Title)
                    .ToList();
            }

            // Yeni filmi isim listesine ekle
            if (!filmNames.Contains(film.Title))
            {
                filmNames.Add(film.Title);
                // Güncellenmiş isim listesini Redis'e kaydet
                _cacheService.Set(filmNamesCacheKey, filmNames);
            }

            // Film eklendikten sonra GetById metodunu çağırarak film detaylarını döndür
            return CreatedAtAction(nameof(GetByName), new { name = film.Title }, filmDto);
        }


     // Film silme
[HttpDelete("name/{name}")]
public IActionResult DeleteByName(string name)
{
    // Film veritabanından isme göre sil
    bool deleteSuccess = _filmRepository.DeleteByName(name);

    if (!deleteSuccess)
    {
        // Silme başarısızsa, 500 Internal Server Error yanıtı döndür
        return StatusCode(500, "Film veritabanından silinemedi.");
    }

    // Redis'ten film bilgilerini kaldır
    string cacheKey = $"film_{name.ToLower()}";
    bool redisDeleteSuccess = _cacheService.Remove(cacheKey);

    if (!redisDeleteSuccess)
    {
        // Redis'ten kaldırma başarısızsa, hata günlüğü yaz ve 500 yanıtı döndür
        _logger.LogError($"Film '{name}' veritabanından silindi ancak Redis'ten silinemedi.");
        return StatusCode(500, "Film veritabanından silindi ancak Redis'ten silinemedi.");
    }

    // Film isimlerini güncelle
    string filmNamesCacheKey = "film_names";
    var filmNames = _cacheService.Get<List<string>>(filmNamesCacheKey);

    if (filmNames != null)
    {
        filmNames.Remove(name);
        _cacheService.Set(filmNamesCacheKey, filmNames);
    }

    return NoContent();
}




    }
}