using System.Diagnostics;

namespace RestProject.Middleware
{
    public class MeasureTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public MeasureTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers["serviceExecutionTime"] = $"{stopwatch.ElapsedMilliseconds} ms";
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
