using DB.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace RestProject.Middleware
{
    public class UserAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            IUserService _userService = userService;
            var username = context.Request.Headers["username"].FirstOrDefault();
            var password = context.Request.Headers["password"].FirstOrDefault();
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                context.Response.Headers["userValidation"] = false.ToString();
            } 
            else
            {
                var userExists = _userService.GetByParameters(username, password).Count != 0;
                context.Response.Headers["userValidation"] = userExists.ToString();
            }
            await _next(context);
        }
    }
}
