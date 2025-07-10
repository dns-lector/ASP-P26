using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ASP_P26.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Entities.UserRole>
    {
        public void Configure(EntityTypeBuilder<Entities.UserRole> builder)
        {
            builder.HasData(
                new Entities.UserRole
                {
                    Id = "SelfRegistered",
                    Description = "Самостійно зареєстрований користувач",
                    CanCreate = false,
                    CanRead   = false,
                    CanUpdate = false,
                    CanDelete = false
                },
                new Entities.UserRole
                {
                    Id = "Employee",
                    Description = "Співробітник компанії",
                    CanCreate = true,
                    CanRead   = true,
                    CanUpdate = false,
                    CanDelete = false
                },
                new Entities.UserRole
                {
                    Id = "Moderator",
                    Description = "Редактор контенту",
                    CanCreate = false,
                    CanRead   = true,
                    CanUpdate = true,
                    CanDelete = true
                },
                new Entities.UserRole
                {
                    Id = "Administrator",
                    Description = "Системний адміністратор",
                    CanCreate = true,
                    CanRead   = true,
                    CanUpdate = true,
                    CanDelete = true
                });
        }
    }
}
