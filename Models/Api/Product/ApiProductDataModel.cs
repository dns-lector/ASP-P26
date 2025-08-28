namespace ASP_P26.Models.Api.Product
{
    public class ApiProductDataModel
    {
        public String  GroupId     { get; set; } = null!;
        public String  Name        { get; set; } = null!;
        public String? Description { get; set; }
        public String? Slug        { get; set; }
        public String? ImageUrl    { get; set; }
        public int     Stock       { get; set; }
        public double  Price       { get; set; }
    }
}
