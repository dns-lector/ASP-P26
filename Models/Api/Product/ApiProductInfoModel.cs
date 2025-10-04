using ASP_P26.Data.Entities;

namespace ASP_P26.Models.Api.Product
{
    public class ApiProductInfoModel
    {
        public String Slug { get; set; } = null!;
        public Data.Entities.Product? Product { get; set; }
        public IEnumerable<Data.Entities.Product> Associations { get; set; } = [];
    }
}
