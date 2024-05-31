using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using DB.Services.Interfaces;
using DB.Entities;
using DB.Dto.User;

namespace RestProject.Filters
{
    public class BasicAuthFilter : IAuthorizationFilter
    {
        private readonly string _realm;
        private readonly IUserService userService;

        public BasicAuthFilter(string realm, IUserService userService)
        {
            _realm = realm;
            this.userService = userService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out StringValues authHeader))
            {
                SetUnauthorizedResult(context);
                return;
            }

            if (StringValues.IsNullOrEmpty(authHeader))
            {
                SetUnauthorizedResult(context);
                return;
            }

            var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader.ToString());

            if (authHeaderVal.Scheme != "Basic")
            {
                SetUnauthorizedResult(context);
                return;
            }

            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderVal.Parameter ?? "")).Split(':');

            if (credentials.Length != 2 || !IsAuthorized(credentials[0], credentials[1]))
            {
                SetUnauthorizedResult(context);
                return;
            }
            // Jeśli użytkownik i hasło są poprawne, można wykonać dodatkową logikę (np. ustawienie kontekstu użytkownika)
            UserDto? user = userService.GetByLogin(credentials[0]);
            context.HttpContext.Items["UserBasicAuthorization"] = user;
        }


        private bool IsAuthorized(string username, string password)
        {
            return userService.Validate(username, password);
        }

        private void SetUnauthorizedResult(AuthorizationFilterContext context)
        {
            context.Result = new UnauthorizedResult();
            context.HttpContext.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_realm}\"";
        }
    }
}
