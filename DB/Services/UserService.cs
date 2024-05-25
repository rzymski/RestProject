using DB.Dto.User;
using DB.Entities;
using DB.Repositories.Interfaces;
using DB.Services.Interfaces;

namespace DB.Services
{
    public class UserService : BaseService<User, UserDto, UserAddEditDto>, IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository repository) : base(repository) 
        { 
            userRepository = repository;
        }

        public override UserDto MapToDto(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User entity cannot be null.");
            return new UserDto(user.Id, user.Login, user.Password, user.Email);
        }

        public override User MapAddEditDtoToEntity(UserAddEditDto dto, User? user = null)
        {
            if (user == null)
                user = new User();

            user.Login= dto.Login;
            user.Password = dto.Password;
            user.Email = dto.Email;

            return user;
        }

        public UserDto? GetByLogin(string login)
        {
            var user = userRepository.GetByLogin(login);
            return user == null ? null : MapToDto(user);
        }

        public override int Add(UserAddEditDto item)
        {
            var existingUser = userRepository.GetByLogin(item.Login);
            if (existingUser != null)
                throw new InvalidOperationException("User with this login already exists.");
            return base.Add(item);
        }

        public override List<int> AddList(List<UserAddEditDto> items)
        {
            var existingLogins = userRepository.GetAll().Select(u => u.Login).ToList();
            var newLogins = items.Select(i => i.Login).ToList();
            var duplicateLogins = newLogins.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateLogins.Any())
                throw new InvalidOperationException($"Duplicate logins found in the list: {string.Join(", ", duplicateLogins)}");
            var existingDuplicates = newLogins.Intersect(existingLogins).ToList();
            if (existingDuplicates.Any())
                throw new InvalidOperationException($"Users with these logins already exist: {string.Join(", ", existingDuplicates)}");
            return base.AddList(items);
        }

        public override bool Update(int id, UserAddEditDto item)
        {
            var existingUser = userRepository.GetByLogin(item.Login);
            if (existingUser != null && existingUser.Id != id)
                throw new InvalidOperationException("Another user with this login already exists.");
            return base.Update(id, item);
        }
    }
}
