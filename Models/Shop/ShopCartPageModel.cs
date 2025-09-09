using ASP_P26.Data.Entities;

namespace ASP_P26.Models.Shop
{
    public class ShopCartPageModel
    {
        public IEnumerable<CartItem>? ActiveCartItems { get; set; }
    }
}
