using ASP_P26.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_P26.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;

        [HttpPost("{id}")]
        public Object AddToCart([FromRoute] String id)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                String? userId = HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;
                if (userId == null)
                {
                    HttpContext.Response.StatusCode = 403;
                    return new { message = "PrimarySid not found" };
                }
                try
                {
                    _dataAccessor.AddToCart(userId, id);
                    return new { message = "Ok" };
                }
                catch (Exception e) when (e is ArgumentNullException || e is FormatException)
                {
                    HttpContext.Response.StatusCode = 400;
                    return new { message = e.Message };
                }
                catch
                {
                    HttpContext.Response.StatusCode = 500;
                    return new { message = "Internal Error" };
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 401;
                return new { message = "UnAuthorized" };
            }
                
        }
    }
}
/* Д.З. Реалізувати відображення статусів відповіді АРІ на запит
 * додавання товару до кошику.
 * - якщо 401, то вивести "Для роботи з кошиком слід увійти" 
 * - якщо інші негативні статуси - "Виникла помилка, оновити сторінку?"
 *     + кнопки (no) і (yes)->оновлення
 */
