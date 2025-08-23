using ASP_P26.Models.Api.Product;
using ASP_P26.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASP_P26.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController(IStorageService storageService) : ControllerBase
    {
        private readonly IStorageService _storageService = storageService;

        [HttpGet]
        public IEnumerable<String> ProductsList()
        {
            return ["1", "2", "3"];
        }

        [HttpPost]
        public async Task<object> CreateProduct(ApiProductFormModel model)
        {
            String savedName;
            try
            {
                // Перевіряємо на дозволеність розширення
                _storageService.TryGetMimeType(model.Image.FileName);
                savedName = await _storageService.SaveItemAsync(model.Image);
            }
            catch(Exception ex)
            {
                return new { status = "Fail", name = ex.Message };
            }
            return new { status = "OK", name = savedName };
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
