using ASP_P26.Data.Entities;

namespace ASP_P26.Models.User
{
    public class UserProfilePageModel
    {
        public bool? IsPersonal { get; set; }
        public String? Name { get; set; }
        public String? Email { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime? RegisteredAt { get; set; }
        public IEnumerable<Cart> Carts { get; set; } = [];
    }
}
