using ASP_P26.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP_P26.Data
{
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        public UserAccess? GetUserAccessByLogin(String userLogin, bool isEditable = false)
        {
            IQueryable<UserAccess> source = _dataContext
                .UserAccesses
                .Include(ua => ua.UserData)
                .Include(ua => ua.UserRole);

            if (isEditable) source = source.AsNoTracking();

            return source.FirstOrDefault(ua => ua.Login == userLogin);
        }

        public void UpdateUserData(UserData userData)
        {

        }
    }
}
