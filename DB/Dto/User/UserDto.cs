using DB.Dto.Base;

namespace DB.Dto.User
{
    public class UserDto : BaseIdDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }

        public UserDto() { }
        public UserDto(int id, string login, string password, string email) : base(id)
        {
            this.Login = login;
            this.Password = password;
            this.Email = email;
        }

        public override string ToString()
        {
            return "UserDto: " + base.ToString();
        }
    }
}
