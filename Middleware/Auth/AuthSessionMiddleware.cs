
using ASP_P26.Data.Entities;
using System.Text.Json;

namespace ASP_P26.Middleware.Auth
{
    public class AuthSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Session.Keys.Contains("userAccess"))
            {
                context.Items["userAccess"] = JsonSerializer
                    .Deserialize<UserAccess>(
                        context.Session.GetString("userAccess")!
                    );
            }
            await _next(context);
        }
    }
}
