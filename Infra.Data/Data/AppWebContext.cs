using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    public class AppWebContext : IdentityDbContext
    {
        #region Constructor
        public AppWebContext(DbContextOptions<AppWebContext> options) : base(options)
        {
        }
        #endregion
        #region DbSets
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<ProductProperty> ProductProperties { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<AttributeTemplate> AttributeTemplates { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<RoleModel> RoleModels { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<UserFavoritesDetail> UserFavoritesDetails { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Question> Question { get; set; }

        #endregion
        #region ModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        #endregion
    }
}
