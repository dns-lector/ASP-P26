using ASP_P26.Data;
using ASP_P26.Data.Entities;
using ASP_P26.Models.Api.Group;
using ASP_P26.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers
{
    [Route("api/product-group")]
    [ApiController]
    public class ProductGroupController(DataAccessor dataAccessor, IStorageService storageService) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IStorageService _storageService = storageService;

        private object AnyRequest()
        {
            String methodName = "Execute" + HttpContext.Request.Method;
            var type = this.GetType();
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
        public IEnumerable<ProductGroup> ExecuteGET()
        {
            return _dataAccessor.GetProductGroups();
        }

        [HttpPost]
        public Object ExecutePOST(ApiGroupFormModel formModel)
        {
            // Валідація моделі
            if (String.IsNullOrEmpty(formModel.Slug))
            {
                return new { status = 400, name = "Slug could not be empty" };
            }
            if (_dataAccessor.IsGroupSlugUsed(formModel.Slug))
            {
                return new { status = 409, name = "Slug is used by other group" };
            }

            String savedName;
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
/* Д.З. Реалізувати повну валідацію моделі товарної групи, у т.ч.
 * ParentId - якщо він є, то повинен бути валідним UUID
 * Одержати статус виконання запиту на створення групи у JS,
 * у випадку успіху очистити форму, у будь-якому випадку видати
 * повідомлення щодо результату.
 */
