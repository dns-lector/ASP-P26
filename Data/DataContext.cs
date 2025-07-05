using ASP_P26.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP_P26.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserData> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserAccess> UserAccesses { get; set; }


        public DataContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserAccess>()
                .HasIndex(ua => ua.Login)
                .IsUnique();

            modelBuilder.Entity<UserAccess>()
                .HasOne(ua => ua.UserData)
                .WithMany(ud => ud.UserAccesses)
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<UserAccess>()
                .HasOne(ua => ua.UserRole)
                .WithMany(ur => ur.UserAccesses)
                .HasForeignKey(ua => ua.RoleId);
        }
    }
}
