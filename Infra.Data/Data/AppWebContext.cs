using Domain.Models;
using Domain.Models.IndexFolder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    public class AppWebContext : IdentityDbContext<ApplicationUser, RoleModel, string>
    {
        public AppWebContext(DbContextOptions<AppWebContext> options) : base(options)
        {
        }

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
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Baner> Baners { get; set; }
        public DbSet<RequestPay> RequestPays { get; set; }
        public DbSet<Factor> Factors { get; set; }
        public DbSet<FactorDetail> FactorDetails { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<CategoryToProduct> CategoryToProducts { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CategoryToProduct>(option =>
            {
                option.HasKey(p => new { p.CategoryId, p.ProductId });
                option.HasOne(p => p.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(p => p.CategoryId);
                option.HasOne(p => p.Product)
                    .WithMany(p => p.Categories)
                    .HasForeignKey(p => p.ProductId);
            });

            builder.Entity<Factor>()
                .HasIndex(factor => factor.CartId)
                .IsUnique();

            builder.Entity<Discount>()
                .HasIndex(discount => discount.CodeName)
                .IsUnique();

            builder.Entity<Cart>()
                .HasIndex(cart => new { cart.UserId, cart.IsFinally });

            builder.Entity<RequestPay>()
                .HasIndex("ApplicationUserId", "IsPay");
        }
    }
}
