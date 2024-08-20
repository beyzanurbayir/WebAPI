public class FilmRepository : IFilmRepository  //IFilmRepository arayüzünü implemente eder
{
    private readonly MovieDbContext _context;   //Veritabanı işlemleri MovieDbContext aracılığıyla yapılır

    public FilmRepository(MovieDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Film> GetAll()
    {
        return _context.Films.ToList();
    }

    public Film GetById(int id)
    {
        return _context.Films.Find(id);
    }

    public void Add(Film film)
    {
        _context.Films.Add(film);
        _context.SaveChanges();
    }

    public void Update(Film film)
    {
        _context.Films.Update(film);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var film = _context.Films.Find(id);
        if (film != null)
        {
            _context.Films.Remove(film);
            _context.SaveChanges();
        }
    }
}
