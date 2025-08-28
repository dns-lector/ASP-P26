using ASP_P26.Data;
using ASP_P26.Models.Shop;
using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Controllers
{
    public class ShopController(DataAccessor dataAccessor) : Controller
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Admin()
        {
            ShopAdminPageModel model = new()
            {
                ProductGroups = 
                    _dataAccessor
                    .GetProductGroups()
                    .Select(g => new Models.OptionModel()
                    {
                        Value = g.Id.ToString(),
                        Content = $"{g.Name} ({g.Description})" 
                    }),
            };
            return View(model);
        }

    }
}
