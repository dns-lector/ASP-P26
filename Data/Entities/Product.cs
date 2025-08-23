using System.ComponentModel.DataAnnotations.Schema;

namespace ASP_P26.Data.Entities
{
    public class Product
    {
        public Guid      Id          { get; set; }
        public Guid?     GroupId     { get; set; }
        public String    Name        { get; set; } = null!;
        public String?   Description { get; set; } 
        public String?   Slug        { get; set; }
        public String?   ImageUrl    { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public double    Price       { get; set; }
        public int       Stock       { get; set; }
        public DateTime? DeletedAt   { get; set; }


        public ProductGroup? Group { get; set; }
    }
}
