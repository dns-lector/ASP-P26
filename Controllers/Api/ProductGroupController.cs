using ASP_P26.Data;
using ASP_P26.Data.Entities;
using ASP_P26.Filters;
using ASP_P26.Models.Api.Group;
using ASP_P26.Models.Rest;
using ASP_P26.Models.Shop;
using ASP_P26.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers.Api
{
    [Route("api/product-group")]
    [ApiController]
    [RestFilter(Name: "Shop API 'product groups'")]
    public class ProductGroupController(DataAccessor dataAccessor, IStorageService storageService) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IStorageService _storageService = storageService;
        private RestResponse restResponse = null!;
        String imgPath => HttpContext.Request.Scheme + "://" +
                    HttpContext.Request.Host + "/Storage/Item/";

        [HttpGet("{id}")]
        public RestResponse GetGroupBySlug(String id)
        {
            if (_dataAccessor.GetProductGroupBySlug(id) is ProductGroup group)
            {
                restResponse.Data = group with
                {
                    ImageUrl = imgPath + group.ImageUrl,
                    Products = [..group.Products.Select(p => p with
                    {
                        ImageUrl = imgPath + p.ImageUrl
                    })]
                };
            }
            else
            {
                restResponse.Status = RestStatus.RestStatus404;
            }
            return restResponse;
        }

        private object AnyRequest()
        {
            // ApiGroupFormModel model = new();
            // TryUpdateModelAsync(model);
            string methodName = "Execute" + HttpContext.Request.Method;
            var type = GetType();
            var action = type.GetMethod(methodName, 
                System.Reflection.BindingFlags.NonPublic 
                | System.Reflection.BindingFlags.Instance);
            if (action == null)
            {
                return new
                {
                    status = 405,
                    message = "Method not Implemented"
                };
            }
            if(HttpContext.Request.Method == "GET")
            {
                return action.Invoke(this, null)!;
            }
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                return new
                {
                    status = 401,
                    message = "UnAuthorized"
                };
            }
            return action.Invoke(this, null)!;
        }

        [HttpGet]
        public RestResponse GetAllGroups()
        {
            restResponse.Meta.ResourceUrl = $"/api/product-group";
            restResponse.Meta.Manipulations = ["GET", "POST"];
            restResponse.Data = new ShopIndexPageModel
            {
                PageTitle = "Крамниця",
                PageTitleImg = imgPath + "logo.jpg",
                ProductGroups = _dataAccessor
                    .GetProductGroups()
                    .Select(g => g with { ImageUrl = imgPath + g.ImageUrl }),
            };
            return restResponse;
            // return _dataAccessor.GetProductGroups();
        }

        [HttpPost]
        public object ExecutePOST(ApiGroupFormModel formModel)
        {
            // Валідація моделі
            if (string.IsNullOrEmpty(formModel.Slug))
            {
                return new { status = 400, name = "Slug could not be empty" };
            }
            if (_dataAccessor.IsGroupSlugUsed(formModel.Slug))
            {
                return new { status = 409, name = "Slug is used by other group" };
            }

            string savedName;
            try
            {
                // Перевіряємо на дозволеність розширення
                _storageService.TryGetMimeType(formModel.Image.FileName);
                savedName = _storageService.SaveItem(formModel.Image);
            }
            catch (Exception ex)
            {
                return new { status = 400, name = ex.Message };
            }

            try
            {
                _dataAccessor.AddProductGroup(new()
                {
                    Name = formModel.Name,
                    Description = formModel.Description,
                    Slug = formModel.Slug,
                    ParentId = formModel.ParentId,
                    ImageUrl = savedName
                });
                return new { status = 201, name = "Created" };
            }
            catch
            {
                return new { status = 500, name = "Error" };
            }
        }
    }
}
/* Д.З. Реалізувати повну валідацію моделі товару.
 * Одержати статус виконання запиту на створення групи у JS,
 * у випадку успіху очистити форму, у будь-якому випадку видати
 * повідомлення щодо результату.
 */
