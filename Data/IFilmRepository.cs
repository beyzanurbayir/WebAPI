public interface IFilmRepository : IRepository<Film>
{
    // Film modeline özgü ek metodlar burada tanımlanabilir.
    // örneğin: Belirli bir türdeki filmleri listelemek için:
    /*
            public interface IFilmRepository : IRepository<Film>
        {
            IEnumerable<Film> GetByGenre(string genre);
        }
    */

    
}
