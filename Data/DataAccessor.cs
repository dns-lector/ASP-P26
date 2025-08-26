using ASP_P26.Data.Entities;
using ASP_P26.Models.Api.Group;
using Microsoft.EntityFrameworkCore;

namespace ASP_P26.Data
{
    public class DataAccessor(DataContext dataContext, ILogger<DataAccessor> logger)
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly ILogger<DataAccessor> _logger = logger;

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
