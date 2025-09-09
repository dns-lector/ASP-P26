using ASP_P26.Data;
using ASP_P26.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_P26.Controllers
{
    public class ShopController(DataAccessor dataAccessor) : Controller
    {
        private readonly DataAccessor _dataAccessor = dataAccessor;

        public IActionResult Index()
        {
            ShopIndexPageModel model = new()
            {
                ProductGroups = _dataAccessor.GetProductGroups(),
            };
            return View(model);
        }

        public IActionResult Group([FromRoute] String id)
        {
            ShopGroupPageModel model = new()
            {
                ProductGroup = _dataAccessor.GetProductGroupBySlug(id),
            };
            return View(model);
        }

        public IActionResult Item([FromRoute] String id)
        {
            ShopItemPageModel model = new()
            {
                Product = _dataAccessor.GetProductBySlug(id),
            };
            return View(model);
        }

        public IActionResult Cart()
        {
            ShopCartPageModel model = new();
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                String userId = HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)!.Value;

                model.ActiveCartItems = _dataAccessor.GetActiveCartItems(userId);            
            }
            return View(model);
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
