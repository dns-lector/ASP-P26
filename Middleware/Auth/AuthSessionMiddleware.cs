
using ASP_P26.Data.Entities;
using System.Security.Claims;
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
            if(context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove("userAccess");
                context.Response.Redirect(context.Request.Path);
                return;
            }
            else if (context.Session.Keys.Contains("userAccess"))
            {
                var ua = JsonSerializer
                    .Deserialize<UserAccess>(
                        context.Session.GetString("userAccess")!
                    )!;

                // context.Items["userAccess"] = ua;
                // Передача даних у вигляді Entities до HttpContext
                // утворює сильне зчеплення, що може призвести до проблем
                // після створення міграцій (змін Entity) або переходу на 
                // іншого постачальника даних

                // Рішення - використання іншої моделі (рівня HttpContext)
                // context.User
                context.User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        new Claim[]
                        {
                            new(ClaimTypes.Name, ua.UserData.Name),
                            new(ClaimTypes.Email, ua.UserData.Email)
                        },
                        nameof(AuthSessionMiddleware)
                    )
                );
            }
            await _next(context);
        }
    }
}

/*
 Browser                                             Server
      HTTP ------------------------------------------>  [WebServer: IIS/Kestrel]
                                                        HttpContext {
POST /Home/Privacy HTTP/1.1                                Request: { 
Host: localhost:1234                                         Method: "POST"
Connection: close                                            Path: "/Home/Privacy" 
Authorization: Basic 2seaoifgnmbv3pe=                        Headers: [ {Connection: close}, ...
Content-Type: application/x-www-form-urlencoded              Body: "x=10&y=20"
                                                           }
x=10&y=20                                                  Response: {...}
                                                           User: null
                                                           Session: null
                                                           WebSocket: null
                                                        }
                                                         | Middleware
                                                       useSession
                                                         |
                                                        HttpContext.Session = ...
                                                         |
                                                       AuthSessionMiddleware
                                                         |
                                                        HttpContext.User = HttpContext.Session[] ...
                                                         |
                                                       return Redirect()
                                                         |
                                                        HttpContext.Response = {
                                                            StatusCode: 302
                                                            Location: "/Home"
                                                         }
                                                     [WebServer: IIS/Kestrel]
      <-------------------------------------------------
                      HTTP/1.1 302 Found
                      Connection: close
                      Location: "/Home"
                      Server: "Kestrel"

 */
