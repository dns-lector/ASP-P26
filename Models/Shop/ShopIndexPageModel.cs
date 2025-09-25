using ASP_P26.Data.Entities;

namespace ASP_P26.Models.Shop
{
    public class ShopIndexPageModel
    {
        public String PageTitle { get; set; } = "";

        public String PageTitleImg { get; set; } = "";

        public IEnumerable<ProductGroup> ProductGroups { get; set; } = [];
    }
}
