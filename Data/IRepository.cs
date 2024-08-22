// Data/IRepository.cs

public interface IRepository<T> where T : class  //Temel CRUD işlemleri için metodları tanımlar
{
    IEnumerable<T> GetAll();
    T GetById(int id);
    void Add(T entity);
    void Update(T entity);
    bool Delete(int id); 
}
