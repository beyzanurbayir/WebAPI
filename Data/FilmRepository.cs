public class FilmRepository : IFilmRepository
{
    private readonly MovieDbContext _context;  

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

    public bool DeleteByName(string name)
    {
        var film = _context.Films.FirstOrDefault(f => f.Title != null && f.Title.ToLower() == name.ToLower());
        if (film == null) return false;

        _context.Films.Remove(film);
        _context.SaveChanges();
        return true;
    }
}
