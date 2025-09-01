using System;
using Domain.Models;
using Domain.Models.IndexFolder;
using Microsoft.AspNetCore.Identity;
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

        public DbSet<Discount> Discounts { get; set; }

        public DbSet<Baner> Baners { get; set; }

        public DbSet<RequestPay> RequestPays { get; set; }

        public DbSet<Factor> Factors { get; set; }
        public DbSet<FactorDetail> FactorDetails { get; set; }

        public DbSet<ContactUs> ContactUs { get; set; }

        public DbSet<CategoryToProduct> CategoryToProducts{ get; set; }

        public DbSet<News> News{ get; set; }

        #endregion
        #region ModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CategoryToProduct>(option =>
            {
                option.HasKey(p => p.CategoryId);
                option.HasKey(p => p.ProductId);
            });

            // Seed Roles
            builder.Entity<RoleModel>().HasData(
                new RoleModel
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    RoleNamePersian = "مدیر سایت"
                },
                new RoleModel
                {
                    Id = "2",
                    Name = "User",
                    NormalizedName = "USER",
                    RoleNamePersian = "کاربر عادی"
                },
                new RoleModel
                {
                    Id = "3",
                    Name = "Seller",
                    NormalizedName = "SELLER",
                    RoleNamePersian = "فروشنده"
                }
            );

            // Seed Default Admin User
            var hasher = new PasswordHasher<ApplicationUser>();
            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "admin@myshop.com",
                    NormalizedUserName = "ADMIN@MYSHOP.COM",
                    Email = "admin@myshop.com",
                    NormalizedEmail = "ADMIN@MYSHOP.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "Admin@123"),
                    SecurityStamp = string.Empty,
                    ConcurrencyStamp = "1",
                    PhoneNumber = "09123456789",
                    PhoneNumberConfirmed = true
                }
            );

            // Add Admin to Admin Role
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "1",
                    UserId = "1"
                }
            );

            // Seed Sample Products
            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Sample Product 1",
                    Detail = "This is a sample product description",
                    Price = 99999, // Price in smallest currency unit (e.g., cents or rial)
                    Count = 100,
                    IsProductHaveAttributes = true,
                    InsertTime = DateTime.Now
                },
                new Product
                {
                    Id = 2,
                    Name = "Sample Product 2",
                    Detail = "Another sample product description",
                    Price = 149999, // Price in smallest currency unit (e.g., cents or rial)
                    Count = 50,
                    IsProductHaveAttributes = false,
                    InsertTime = DateTime.Now
                }
            );

            base.OnModelCreating(builder);
        }
        #endregion
    }
}
