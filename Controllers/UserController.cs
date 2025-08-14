using ASP_P26.Data;
using ASP_P26.Data.Entities;
using ASP_P26.Models.User;
using ASP_P26.Services.Email;
using ASP_P26.Services.Jwt;
using ASP_P26.Services.Kdf;
using ASP_P26.Services.Random;
using ASP_P26.Services.Time;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using System.Security.Claims;
using System.Text.Json;

namespace ASP_P26.Controllers
{
    public class UserController(
        IRandomService randomService,
        DataContext dataContext,
        ILogger<UserController> logger,
        ITimeService timeService,
        IEmailService emailService,
        IJwtService jwtService,
        IKdfService kdfService) : Controller
    {
        private readonly IRandomService _randomService = randomService;
        private readonly IKdfService _kdfService = kdfService;
        private readonly DataContext _dataContext = dataContext;
        private readonly ILogger<UserController> _logger = logger;
        private readonly ITimeService _timeService = timeService;
        private readonly IEmailService _emailService = emailService;
        private readonly IJwtService _jwtService = jwtService;

        [HttpPost]
        public JsonResult Email()
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                // try
                // {
                //     _emailService.Send(
                //         "azure.spd111.od.0@ukr.net",
                //         "ASP - P26",
                //         "Hello, World!");
                // }
                // catch (Exception ex)
                // {
                //     return Json(new
                //     {
                //         Status = 500,
                //         Data = ex.Message
                //     });
                // }
                
                return Json(new
                {
                    Status = 200,
                    Data = "Ok"
                });
            }
            else
            {
                return Json(new
                {
                    Status = 401,
                    Data = "UnAuthorized"
                });
            }
        }

        private UserAccess Authenticate()
        {
            // Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            String authHeader = Request.Headers.Authorization.ToString();  // Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            if (String.IsNullOrEmpty(authHeader))
            {
                throw new Exception("Missing 'Authorization' header");
            }
            String authScheme = "Basic ";
            if (!authHeader.StartsWith(authScheme))
            {
                throw new Exception($"Authorization scheme error: '{authScheme}' only");
            }
            String credentials = authHeader[authScheme.Length..];  // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            String decoded;
            try
            {
                decoded = System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(credentials));
            }
            catch (Exception ex)
            {
                _logger.LogError("SignIn: {ex}", ex.Message);
                throw new Exception($"Authorization credentials decode error");
            }
            String[] parts = decoded.Split(':', 2);
            if (parts.Length != 2)
            {
                throw new Exception($"Authorization credentials decompose error");
            }
            String login = parts[0];
            String password = parts[1];
            var userAccess = _dataContext
                .UserAccesses
                .AsNoTracking()
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole)
                .FirstOrDefault(ua => ua.Login == login);

            if (userAccess == null)
            {
                throw new Exception($"Authorization credentials rejected");
            }
            if (_kdfService.Dk(password, userAccess.Salt) != userAccess.Dk)
            {
                throw new Exception($"Authorization credentials rejected.");
            }
            return userAccess;
        }

        [HttpGet]
        public JsonResult LogIn()
        {
            UserAccess userAccess;
            try
            {
                userAccess = Authenticate();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = 401,
                    Data = ex.Message
                });
            }
            // Токени.
            // цифрові "посвідчення", що несуть інформацію про користувача
            // За прямою наявністю інформації токени поділяють на 
            //  JWT - з наявністю інформації
            //  Bearer - лише з ідентифікатором токена

            // створюємо новий токен
            AccessToken accessToken = new()
            {
                Jti = Guid.NewGuid().ToString(),
                Sub = userAccess.Id,
                Iat = _timeService.Timestamp().ToString(),
                Exp = (_timeService.Timestamp() + (long)1e5).ToString(),
                Iss = nameof(ASP_P26),
                Aud = userAccess.RoleId
            };

            _dataContext.AccessTokens.Add(accessToken);
            _dataContext.SaveChanges();

            var jwt = new
            {
                accessToken.Jti,
                accessToken.Sub,
                accessToken.Iat,
                accessToken.Exp,
                accessToken.Iss,
                accessToken.Aud,
                userAccess.UserData.Name,
                userAccess.UserData.Email,
            };

            return Json(new
            {
                Status = 200,
                Data = _jwtService.EncodeJwt(jwt)
            });
        }

        [HttpGet]
        public JsonResult SignIn()
        {
            UserAccess userAccess;
            try
            {
                userAccess = Authenticate();
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = 401,
                    Data = ex.Message
                });
            }
            HttpContext.Session.SetString("userAccess",
                JsonSerializer.Serialize(userAccess));

            return Json(new {
                Status = 200,
                Data = "OK"
            });
        }

        public ViewResult Profile(String id)
        {
            UserProfilePageModel model = new();
            var ua = _dataContext  // визначаємо чий профіль запитано
                .UserAccesses
                .AsNoTracking()
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole)
                .FirstOrDefault(ua => ua.Login == id);
            if (ua == null)
            {
                model.IsPersonal = null;
            }
            else
            {
                model.Name = ua.UserData.Name;
                model.RegisteredAt = ua.UserData.RegisteredAt;

                bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
                if (isAuthenticated)
                {
                    model.Email = ua.UserData.Email;

                    // дістаємо свій логін (з яким автентифіковані)
                    String userLogin = HttpContext
                        .User
                        .Claims
                        .First(c => c.Type == ClaimTypes.Sid)
                        .Value;                   

                    if (ua.Login == userLogin)  // Перегляд свого профілю
                    {
                        model.IsPersonal = true;
                        model.Birthdate = ua.UserData.Birthdate;
                    }
                    else  // Перегляд профілю іншого користувача
                    {
                        model.IsPersonal = false;
                    }                    
                }
                else  // Перегляд невідомого профілю у неавторизованому режимі
                {
                    model.IsPersonal = false;
                }
            }                
            return View(model);
        }

        public ViewResult SignUp()
        {
            UserSignupPageModel pageModel = new();
            if (HttpContext.Session.Keys.Contains("UserSignupFormModel"))
            {
                pageModel.FormModel = JsonSerializer.Deserialize<UserSignupFormModel>(
                    HttpContext.Session.GetString("UserSignupFormModel")!
                );
                pageModel.FormErrors = ProcessSignupData(pageModel.FormModel!);
                HttpContext.Session.Remove("UserSignupFormModel");
            }
            return View(pageModel);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Register(UserSignupFormModel model)
        {
            HttpContext.Session.SetString(
                "UserSignupFormModel",
                JsonSerializer.Serialize(model)
            );
            return RedirectToAction(nameof(SignUp));
        }

        private Dictionary<String,String> ProcessSignupData(UserSignupFormModel model)
        {
            Dictionary<String, String> errors = [];

            #region Validation
            if (String.IsNullOrEmpty(model.UserName))
            {
                errors[nameof(model.UserName)] = "Ім'я не може бути порожнім";
            }
            if (String.IsNullOrEmpty(model.UserEmail))
            {
                errors[nameof(model.UserEmail)] = "E-mail не може бути порожнім";
            }
            if (String.IsNullOrEmpty(model.UserLogin))
            {
                errors[nameof(model.UserLogin)] = "Логін не може бути порожнім";
            }
            else
            {
                if(model.UserLogin.Contains(':'))
                {
                    errors[nameof(model.UserLogin)] = "Логін не може містити символ ':'";
                }
                else
                {
                    if(_dataContext.UserAccesses.Any(ua => ua.Login == model.UserLogin))
                    {
                        errors[nameof(model.UserLogin)] = "Логін у вжитку";
                    }
                }
            }
            // Решта перевірок

            #endregion
            
            if(errors.Count == 0)
            {
                Guid userId = Guid.NewGuid();

                UserData user = new()
                {
                    Id = userId,
                    Name = model.UserName,
                    Email = model.UserEmail,
                    Birthdate = model.Birthdate,
                    RegisteredAt = DateTime.Now,
                };
                String salt = _randomService.Otp(12);
                UserAccess userAccess = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Login = model.UserLogin,
                    Salt = salt,
                    Dk = _kdfService.Dk(model.UserPassword, salt),
                    RoleId = "SelfRegistered"
                };
                // додаємо нові об'єкти до контексту
                _dataContext.Database.BeginTransaction();
                _dataContext.Users.Add(user);
                _dataContext.UserAccesses.Add(userAccess);
                // після додавання даних до контексту вони доступні у програмі, але
                // не передані до БД
                try
                {
                    _dataContext.SaveChanges();
                    _dataContext.Database.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError("ProcessSignupData: {ex}", ex.Message);
                    _dataContext.Database.RollbackTransaction();
                    errors["500"] = "Проблема зі збереженням. Спробуйте пізніше.";
                }
            }
            return errors;
        }
    }
}
