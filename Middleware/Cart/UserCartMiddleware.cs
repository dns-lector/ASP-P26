using ASP_P26.Data;
using ASP_P26.Middleware.Auth;
using ASP_P26.Models;
using ASP_P26.Services.Jwt;
using System.Security.Claims;

namespace ASP_P26.Middleware.Cart
{
    public class UserCartMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;
        public async Task InvokeAsync(
                HttpContext context,
                DataAccessor dataAccessor)
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                String userId = context.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)!.Value;

                context.Items.Add(
                    nameof(UserCartModel), 
                    new UserCartModel
                    {
                        ActiveCart = dataAccessor.GetActiveCart(userId)
                    });
            }
            await _next(context);
        }
    }


    public static class UserCartMiddlewareExtension
    {
        public static IApplicationBuilder UseUserCart(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserCartMiddleware>();
        }
    }
}
