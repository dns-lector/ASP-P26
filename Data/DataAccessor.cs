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

            if (isEditable) source = source.AsNoTracking();

            return source.FirstOrDefault(ua => 
                ua.Login == userLogin && 
                ua.UserData.DeletedAt == null);
        }

        public void UpdateUserData(UserData userData)
        {

        }
    }
}
