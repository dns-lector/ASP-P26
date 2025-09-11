using ASP_P26.Data.Entities;
using ASP_P26.Models.Api.Group;
using ASP_P26.Models.Api.Product;
using Microsoft.EntityFrameworkCore;

namespace ASP_P26.Data
{
    public class DataAccessor(DataContext dataContext, ILogger<DataAccessor> logger)
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly ILogger<DataAccessor> _logger = logger;

        public void AddToCart(String userId, String productId)
        {
            Guid userGuid = Guid.Parse(userId);
            Guid productGuid = Guid.Parse(productId);
            var user = _dataContext.Users.Find(userGuid) ??
                throw new ArgumentException("user not found", nameof(userId));
            Product? product = _dataContext.Products.Find(productGuid) ??
                throw new ArgumentException("product not found", nameof(productId));
            // Якщо у користувача є відкритий кошик, то
            //   якщо у кошику є такий товар, то збільшуємо його кількість
            //   інакше створюємо новий запис CartItem
            // інакше створюємо новий кошик і додаємо товар
            var cart = _dataContext
                .Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userGuid
                    && c.PaidAt == null && c.DeletedAt == null);
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Price = 0,
                    UserId = userGuid,
                };
                _dataContext.Carts.Add(cart);
            }

            CartItem? cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productGuid);
            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    Price = product.Price,
                    Quantity = 1,
                    ProductId = productGuid,
                };
                // cart.CartItems.Add(cartItem);
                _dataContext.CartItems.Add(cartItem);
                cart.Price += cartItem.Price;   // TODO: DiscountService
            }
            else
            {
                cartItem.Quantity += 1;
                cartItem.Price += product.Price;
                cart.Price += product.Price;     // TODO: DiscountService
            }
            _dataContext.SaveChanges();
        }
        public IEnumerable<CartItem> GetActiveCartItems(String userId)
        {            
            var cart = GetActiveCart(userId);
            return cart?.CartItems ?? [];
        }
        public IEnumerable<Cart> GetCarts()
        {
            return [];
        }
        public Cart? GetActiveCart(String userId, bool isEditable = false)
        {
            Guid userGuid = Guid.Parse(userId);

            var user = _dataContext.Users.Find(userGuid) ??
                throw new ArgumentException("user not found", nameof(userId));

            IQueryable<Cart> source = _dataContext
               .Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product);

            if (!isEditable) source = source.AsNoTracking();

            return source.FirstOrDefault(c => c.UserId == userGuid
                    && c.PaidAt == null && c.DeletedAt == null);
        }
        public void ModifyCart(String userId, String productId, int increment)
        {
            Guid userGuid = Guid.Parse(userId);
            Guid productGuid = Guid.Parse(productId);
            var user = _dataContext.Users.Find(userGuid) ??
                throw new ArgumentException("user not found", nameof(userId));
            Cart cart = GetActiveCart(userId, isEditable: true) ??
                throw new ArgumentException("active cart not found");

            // Якщо у кошику немає заданого товару, то це неправильний запит
            CartItem cartItem = cart
                .CartItems
                .FirstOrDefault(ci => ci.ProductId == productGuid)
                ?? throw new ArgumentException("product not found", nameof(productId));
            
            // Якщо інкремент від'ємний і призводить до від'ємного підсумку,
            // то це неправильний запит
            int newQuantity = cartItem.Quantity + increment;
            if(newQuantity < 0)
            {
                throw new ArgumentException("increment causes negative quantity");
            }
            // Якщо інкремент перевищує наявність товару, то це необроблюваний запит
            if(newQuantity > cartItem.Product.Stock)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (newQuantity == 0)
            {
                // Delete
                _dataContext.CartItems.Remove(cartItem);
                cart.Price -= cartItem.Price;
            }
            else
            {
                cartItem.Quantity = newQuantity;
                // TODO: DiscountService
                cartItem.Price += increment * cartItem.Product.Price;
                cart.Price += increment * cartItem.Product.Price;
            }
            _dataContext.SaveChanges();
        }
        public void CheckoutActiveCart(String userId)
        {
            Cart cart = GetActiveCart(userId, isEditable: true)
                ?? throw new ArgumentException("active cart not found");
            cart.PaidAt = DateTime.Now;
            _dataContext.SaveChanges();
        }
        public void DiscardActiveCart(String userId)
        {
            Cart cart = GetActiveCart(userId, isEditable: true)
                ?? throw new ArgumentException("active cart not found");
            cart.DeletedAt = DateTime.Now;
            _dataContext.SaveChanges();
        }

        public bool IsProductSlugUsed(String slug)
        {
            return _dataContext.Products.Any(g => g.Slug == slug);
        }
        public void AddProduct(ApiProductDataModel model)
        {
            Guid groupId;
            try { groupId = Guid.Parse(model.GroupId); }
            catch { throw; }

            _dataContext.Products.Add(new()
            {
                GroupId = groupId,
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug,
                ImageUrl = model.ImageUrl,
                Price = model.Price,
                Stock = model.Stock,
                DeletedAt = null,
            });
            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError("AddProduct: {ex}", ex.Message);
                throw;
            }
        }
        public Product? GetProductBySlug(String slug)
        {
            return _dataContext
                .Products
                .AsNoTracking()
                .FirstOrDefault(p => 
                    (p.Slug == slug || p.Id.ToString() == slug)
                    && p.DeletedAt == null
                );
        }

        public bool IsGroupSlugUsed(String slug)
        {
            return _dataContext.ProductGroups.Any(g => g.Slug == slug);
        }
        public void AddProductGroup(ApiGroupDataModel model)
        {
            _dataContext.ProductGroups.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Slug = model.Slug,
                ImageUrl = model.ImageUrl,
                ParentId = model.ParentId == null ? null : Guid.Parse(model.ParentId),
                DeletedAt = null,
            });
            try
            {
                _dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError("AddProductGroup: {ex}", ex.Message);
                throw;
            }
        }
        public IEnumerable<ProductGroup> GetProductGroups()
        {
            return _dataContext
                .ProductGroups
                .AsNoTracking()
                .Where(g => g.DeletedAt == null)
                .AsEnumerable();
        }
        public ProductGroup? GetProductGroupBySlug(String slug)
        {
            return _dataContext
            .ProductGroups
            .Include(g => g.Products)
            .AsNoTracking()
            .FirstOrDefault(g => g.Slug == slug && g.DeletedAt == null);
        }


        public async Task<bool> DeleteUserAsync(String userLogin)
        {
            UserAccess? ua = await _dataContext
                .UserAccesses
                .Include(ua => ua.UserData)
                .FirstOrDefaultAsync(ua => ua.Login == userLogin);
            if (ua == null)
            {
                return false;
            }
            ua.UserData.Name = "";
            ua.UserData.Email = "";
            ua.UserData.Birthdate = null;
            ua.UserData.DeletedAt = DateTime.Now;
            try
            {
                await _dataContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("DeleteUserAsync: {ex}", ex.Message);
                return false;
            }
        }

        public UserAccess? GetUserAccessByLogin(String userLogin, bool isEditable = false)
        {
            IQueryable<UserAccess> source = _dataContext
                .UserAccesses
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole);

            if (!isEditable) source = source.AsNoTracking();

            return source.FirstOrDefault(ua => 
                ua.Login == userLogin && 
                ua.UserData.DeletedAt == null);
        }

        public void UpdateUserData(UserData userData)
        {

        }
    }
}
