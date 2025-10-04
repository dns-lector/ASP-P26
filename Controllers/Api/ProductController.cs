using ASP_P26.Data;
using ASP_P26.Filters;
using ASP_P26.Models.Api.Product;
using ASP_P26.Models.Rest;
using ASP_P26.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASP_P26.Controllers.Api
{
    [Route("api/product")]
    [ApiController]
    //[AuthorizationFilter]
    public class ProductController(DataAccessor dataAccessor, IStorageService storageService) : ControllerBase
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private readonly IStorageService _storageService = storageService;
        String imgPath => HttpContext.Request.Scheme + "://" +
                    HttpContext.Request.Host + "/Storage/Item/";

        [HttpGet("{id}")]
        public RestResponse ProductInfo(String id)
        {
            ApiProductInfoModel model = new()
            {
                Slug = id,
                Product = _dataAccessor.GetProductBySlug(id)
            };
            if (model.Product != null)
            {
                model.Product = model.Product with
                {
                    ImageUrl = imgPath + model.Product.ImageUrl,
                };
                model.Associations = _dataAccessor
                    .GetProductAssociations(model.Product)
                    .Select(p => p with
                    {
                        ImageUrl = imgPath + p.ImageUrl,
                    });
            }
            RestResponse restResponse = new()
            {
                Status = model.Product != null 
                    ? RestStatus.RestStatus200 
                    : RestStatus.RestStatus404,
                Meta = new()
                {
                    ResourceName = "Shop API 'Product'. Get info by slug/id",
                    Cache = 86400,
                    Method = "GET",
                    DataType = "json/oblect",
                    ServerTime = DateTime.Now.Ticks,
                },
                Data = model
            };
            return restResponse;
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
