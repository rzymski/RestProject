using DB.Entities;
using DB.Repositories.Interfaces;

namespace DB.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(MyDBContext dbContext) : base(dbContext)
        {
        }
    }
}
