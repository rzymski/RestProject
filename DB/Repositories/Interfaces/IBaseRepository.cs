namespace DB.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        List<T> GetAll();
        T? GetById(int id);
        void Add(T entity);
        void AddList(List<T> entities);
        void Update(T entity);
        void Delete(int id);
    }
}
