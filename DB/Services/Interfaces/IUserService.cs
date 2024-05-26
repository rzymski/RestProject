using DB.Dto.FlightReservation;
using DB.Dto.User;
using DB.Entities;

namespace DB.Services.Interfaces
{
    public interface IUserService : IBaseService<User, UserDto, UserAddEditDto>
    {
        UserDto? GetByLogin(string login);
        List<UserDto> GetByParameters(string? login, string? password, string? email = null);
    }
}
