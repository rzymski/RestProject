using DB.Dto.Base;

namespace DB.Dto.User
{
    public class UserAddEditDto : BaseDto
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }

        public UserAddEditDto() { }
        public UserAddEditDto(string login, string password, string email)
        {
            this.Login = login;
            this.Password = password;
            this.Email = email;
        }

        public override string ToString()
        {
            return "UserAddEditDto: " + base.ToString();
        }
    }
}
