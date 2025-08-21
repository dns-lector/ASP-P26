using ASP_P26.Models.Api.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ASP_P26.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<String> ProductsList()
        {
            return ["1", "2", "3"];
        }

        [HttpPost]
        public async Task<object> CreateProduct(ApiProductFormModel model)
        {
            // Виділяємо з імені файлу розширення (воно буде збережене у імені)
            String ext = model.Image.FileName[model.Image.FileName.LastIndexOf('.')..];
            // Генеруємо нове ім'я для збереження файлу
            String savedName = Guid.NewGuid() + ext;
            String path = "C:/storage/ASP26/" + savedName;
            // Відкриваємо потік для збереження (не забуваємо автозакриття)
            using Stream stream = new StreamWriter(path).BaseStream;
            var copyTask = model.Image.CopyToAsync(stream);
            // вносимо дані до БД

            await copyTask;
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
