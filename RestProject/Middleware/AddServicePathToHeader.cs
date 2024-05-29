using Microsoft.AspNetCore.Http.Extensions;

namespace RestProject.Middleware
{
    public class AddServicePathToHeader
    {
        private readonly RequestDelegate _next;

        public AddServicePathToHeader(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IHttpContextAccessor httpContextAccessor)
        {
            IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
            if(httpContextAccessor.HttpContext != null)
                context.Response.Headers["servicePath"] = httpContextAccessor.HttpContext.Request.GetEncodedUrl();
            await _next(context);
        }
    }
}
