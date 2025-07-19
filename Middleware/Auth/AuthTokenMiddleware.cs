using ASP_P26.Data;
using ASP_P26.Data.Entities;
using ASP_P26.Middleware.Auth;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace ASP_P26.Middleware.Auth
{
    public class AuthTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DataContext dataContext)
        {
            if (context
                .Request
                .Headers
                .Authorization
                .FirstOrDefault(h =>  h?.StartsWith("Bearer ") ?? false)
                is String authHeader)
            {
                String jti = authHeader[7..];
                if (dataContext
                    .AccessTokens
                    .AsNoTracking()
                    .Include(at => at.UserAccess)
                        .ThenInclude(ua => ua.UserData)
                    .FirstOrDefault(at => at.Jti == jti)
                    is AccessToken accessToken)
                {
                    /* Д.З. Реалізувати перевірку токена авторизації
                     * на термін придатності з боку сервера (AuthTokenMiddleware)
                     * Особливість - у користувача може бути декілька токенів,
                     * пошук має бути для "наймолодшого".
                     */
                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            new Claim[]
                            {
                                new(ClaimTypes.Name, accessToken.UserAccess.UserData.Name),
                                new(ClaimTypes.Email, accessToken.UserAccess.UserData.Email)
                            },
                            nameof(AuthTokenMiddleware)
                        )
                    );
                }
            }
            await _next(context);
        }
    }

    public static class AuthTokenMiddlewareExtension
    {
        public static IApplicationBuilder UseAuthToken(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthTokenMiddleware>();
        }
    }
}
