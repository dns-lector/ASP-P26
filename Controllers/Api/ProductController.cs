using ASP_P26.Data;
using ASP_P26.Filters;
using ASP_P26.Models.Api.Product;
using ASP_P26.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASP_P26.Controllers.Api
{
    [Route("api/product")]
    [ApiController]
    [AuthorizationFilter]
    public class ProductController(DataAccessor dataAccessor, IStorageService storageService) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IStorageService _storageService = storageService;

        [HttpGet]
        public IEnumerable<string> ProductsList()
        {
            return ["1", "2", "3"];
        }

        [HttpPost]
        public async Task<object> CreateProduct(ApiProductFormModel formModel)
        {
            // Валідація моделі
            if (formModel.Slug != null)
            {
                if (_dataAccessor.IsProductSlugUsed(formModel.Slug))
                {
                    return new { status = 409, name = "Slug is used by other group" };
                }
            }

            string? savedName = null;
            if (formModel.Image != null)
            {
                try
                {
                    // Перевіряємо на дозволеність розширення
                    _storageService.TryGetMimeType(formModel.Image.FileName);
                    savedName = await _storageService.SaveItemAsync(formModel.Image);
                }
                catch (Exception ex)
                {
                    return new { status = 400, name = ex.Message };
                }
            }

            try
            {
                _dataAccessor.AddProduct(new()
                {
                    GroupId = formModel.GroupId,
                    Name = formModel.Name,
                    Description = formModel.Description,
                    Slug = formModel.Slug,
                    ImageUrl = savedName,
                    Price = formModel.Price,
                    Stock = formModel.Stock,
                });
                return new { status = 201, name = "Created" };
            }
            catch (Exception e) when (e is ArgumentNullException || e is FormatException)
            {
                return new { status = 400, name = e.Message };
            }
            catch
            {
                return new { status = 500, name = "Error" };
            }
        }
    }
}

/* Відмінності MVC та API контролерів
 * MVC: один метод запиту (частіше за все GET) та різні адреси
 * GET /home/privacy  -> HomeController::Privacy()
 * POST /home/index  -> HomeController::Index()
 * 
 * API - одна адреса, різні методи запиту
 * GET  /api/product -> ProductController::ProductsList()
 * POST /api/product -> ProductController::CreateProduct()
 * PUT  /api/product
 * --------------------------------------------------------
 * MVC - повертають IActionResult
 * API - об'єкти довільного типу, які далі ASP переводить до JSON 
 *        (окрім String, що переводиться до text/plain)
 */
