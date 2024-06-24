using API.Data.Configurations;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
        IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            new AppUserEntityConfiguration().Configure(builder.Entity<AppUser>());
            new AppRoleEntityConfiguration().Configure(builder.Entity<AppRole>());

            new UserLikeEntityConfiguration().Configure(builder.Entity<UserLike>());
            new MessageEntityConfiguration().Configure(builder.Entity<Message>());
        }

    }
}
