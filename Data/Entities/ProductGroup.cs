namespace ASP_P26.Data.Entities
{
    public class ProductGroup
    {
        public Guid      Id          { get; set; }
        public Guid?     ParentId    { get; set; }
        public String    Name        { get; set; } = null!;
        public String    Description { get; set; } = null!;
        public String    Slug        { get; set; } = null!;
        public String    ImageUrl    { get; set; } = null!;
        public DateTime? DeletedAt   { get; set; }

        public ProductGroup? ParentGroup { get; set; }
        public IEnumerable<Product> Products { get; set; } = [];
    }
}
