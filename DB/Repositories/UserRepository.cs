using DB.Entities;
using DB.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DB.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(MyDBContext dbContext) : base(dbContext) {}

        public User GetByLogin(string login)
        {
            return _dbContext.Set<User>().SingleOrDefault(u => u.Login == login);
        }
    }
}
