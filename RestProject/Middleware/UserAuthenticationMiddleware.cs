using DB.Dto.User;
using DB.Entities;
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

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password)) 
            {
                UserDto? user = _userService.GetByParameters(username, password).FirstOrDefault();
                if (user != null)
                {
                    context.Response.Headers["userValidation"] = true.ToString();
                    context.Items["UserAuthenticationMiddleware"] = user;
                    await _next(context);
                    return;
                }
            }

            context.Response.Headers["userValidation"] = false.ToString();
            context.Items["UserAuthenticationMiddleware"] = null;
            await _next(context);
        }
    }
}
