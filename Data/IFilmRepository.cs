public interface IFilmRepository
{
    IEnumerable<Film> GetAll();
    Film GetById(int id);
    void Add(Film film);
    void Update(Film film);
    bool DeleteByName(string name);
}
