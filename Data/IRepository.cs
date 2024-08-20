// Amaç: Veritabanı işlemlerinin genel metodlarını tanımlayarak, veri erişim katmanını soyutlamak.

public interface IRepository<T> where T : class  //Temel CRUD işlemleri için metodları tanımlar
{
    IEnumerable<T> GetAll();
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
}
