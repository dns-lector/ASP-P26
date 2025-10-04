using ASP_P26.Data;
using ASP_P26.Filters;
using ASP_P26.Models.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace ASP_P26.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    [RestFilter(Name: "Shop API 'user cart'")]
    public class CartController(
        DataAccessor dataAccessor, 
        ILogger<CartController> logger) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly ILogger<CartController> _logger = logger;
        private RestResponse restResponse = null!;
        String imgPath => HttpContext.Request.Scheme + "://" +
                   HttpContext.Request.Host + "/Storage/Item/";

        [HttpGet]
        public RestResponse ActiveCart()
        {
            restResponse.Meta.ResourceUrl = $"/api/cart";
            restResponse.Meta.Manipulations = ["PUT", "DELETE"];
            ExecuteAuthority(
               (u) => {
                   var activeCart = _dataAccessor.GetActiveCart(u);
                   if (activeCart != null) 
                   {
                       activeCart = activeCart with
                       {
                           CartItems = [..activeCart.CartItems.Select(ci =>
                               ci with { 
                                   Product = ci.Product with { 
                                       ImageUrl = imgPath + ci.Product.ImageUrl 
                                } 
                           })]
                       };
                   }
                   restResponse.Data = activeCart;
                },
               nameof(ActiveCart)
               );
            return restResponse;
        }

        [HttpDelete]
        public RestResponse DiscardCart()
        {
            restResponse.Meta.ResourceUrl = $"/api/cart";
            restResponse.Meta.Manipulations = ["PUT", "DELETE"];

            ExecuteAuthority(
               _dataAccessor.DiscardActiveCart,
               nameof(DiscardCart));

            return restResponse;
        }

        [HttpPut]
        public RestResponse CheckoutCart()
        {
            restResponse.Meta.ResourceUrl = $"/api/cart";
            restResponse.Meta.Manipulations = ["PUT", "DELETE"];

            ExecuteAuthority(
               // (userId) => _dataAccessor.CheckoutActiveCart(userId),
               _dataAccessor.CheckoutActiveCart,
               nameof(CheckoutCart));           

            return restResponse;
        }

        [HttpPost("{id}")]
        public RestResponse AddToCart([FromRoute] String id)
        {
            restResponse.Meta.ResourceUrl = $"/api/cart/{id}";
            restResponse.Meta.Manipulations = ["POST", "PATCH", "DELETE"];

            ExecuteAuthority(
                (userId) => _dataAccessor.AddToCart(userId, id), 
                nameof(AddToCart));

            return restResponse;
        }

        [HttpPatch("{id}")]
        public RestResponse ChangeCart(String id, int increment)
        {
            restResponse.Meta.ResourceUrl = $"/api/cart/{id}";
            restResponse.Meta.Manipulations = ["POST", "PATCH", "DELETE"];
            
            ExecuteAuthority(
                (userId) => _dataAccessor.ModifyCart(userId, id, increment),
                nameof(ChangeCart));

            return restResponse;
        }


        [HttpPost("repeat/{id}")]
        public RestResponse RepeatCart(String id)
        {
            restResponse.Data = id;
            return restResponse;
        }

        private void ExecuteAuthority(Action<String> action, String callerName)
        {
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
                        action(userId);
                    }
                    catch (Exception e) when (e is ArgumentException || e is ArgumentNullException || e is FormatException)
                    {
                        restResponse.Status = RestStatus.RestStatus400;
                        restResponse.Data = e.Message;
                        restResponse.Meta.DataType = "string";
                    }
                    catch (Exception e)
                    {
                        restResponse.Status = RestStatus.RestStatus500;
                        _logger.LogError("{callerName} {ex}", callerName, e.Message);
                    }
            }
            else
            {
                restResponse.Status = RestStatus.RestStatus401;
            }
        }
    }
}
/* Д.З. Реалізувати дію кнопки видалення всього кошику
 * попередньо запитавши підтвердження "Дія не може бути скасована.
 * Видалити кошик з 3 товарами на суму 1231.49 грн?"
 * Після успішного видалення переводити на головну сторінку крамниці.
 */
