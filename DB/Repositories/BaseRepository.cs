using DB.Entities;
using DB.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly MyDBContext _dbContext;

        public BaseRepository(MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<T> GetAll()
        {
            return _dbContext.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _dbContext.Set<T>().SingleOrDefault(x => x.Id == id);
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var item = _dbContext.Set<T>().SingleOrDefault(x => x.Id == id);
            if (item is null) return;
            _dbContext.Set<T>().Remove(item);
            _dbContext.SaveChanges();
        }
    }
}
