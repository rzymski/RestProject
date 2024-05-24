using DB.Entities;

namespace DB.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User GetByLogin(string login);
    }
}
