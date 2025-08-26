using Microsoft.AspNetCore.Mvc;

namespace ASP_P26.Models.Api.Group
{
    public class ApiGroupDataModel
    {
        public String  Name { get; set; } = null!;
        public String  Description { get; set; } = null!;
        public String  Slug { get; set; } = null!;
        public String? ParentId { get; set; }
        public String  ImageUrl { get; set; } = null!;
    }
}
