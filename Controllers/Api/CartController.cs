using ASP_P26.Data;
using ASP_P26.Models.Rest;
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
        public RestResponse AddToCart([FromRoute] String id)
        {
            RestResponse restResponse = new();
            restResponse.Meta.ResourceName = "Shop API 'user cart'";
            restResponse.Meta.ResourceUrl = $"/api/cart/{id}";
            restResponse.Meta.Method = "POST";
            restResponse.Meta.Manipulations = ["POST", "PATCH", "DELETE"];
            
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                String? userId = HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;
                if (userId == null)
                {
                    restResponse.Status = RestStatus.RestStatus403;
                    restResponse.Data = "PrimarySid not found";
                    restResponse.Meta.DataType = "string";
                }
                else try
                    {
                        _dataAccessor.AddToCart(userId, id);
                    }
                    catch (Exception e) when (e is ArgumentNullException || e is FormatException)
                    {
                        restResponse.Status = RestStatus.RestStatus400;
                        restResponse.Data = e.Message;
                        restResponse.Meta.DataType = "string";
                    }
                    catch
                    {
                        restResponse.Status = RestStatus.RestStatus500;
                    }
            }
            else
            {
                restResponse.Status = RestStatus.RestStatus401;
            }
            return restResponse;
        }
    }
}
/* Д.З. Реалізувати відображення статусів відповіді АРІ на запит
 * додавання товару до кошику.
 * - якщо 401, то вивести "Для роботи з кошиком слід увійти" 
 * - якщо інші негативні статуси - "Виникла помилка, оновити сторінку?"
 *     + кнопки (no) і (yes)->оновлення
 */
