using ASP_P26.Data.Entities;

namespace ASP_P26.Models.Shop
{
    public class ShopIndexPageModel
    {
        public IEnumerable<ProductGroup> ProductGroups { get; set; } = [];
    }
}
