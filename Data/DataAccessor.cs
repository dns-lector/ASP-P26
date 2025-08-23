using ASP_P26.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP_P26.Data
{
    public class DataAccessor(DataContext dataContext, ILogger<DataAccessor> logger)
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly ILogger<DataAccessor> _logger = logger;

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
