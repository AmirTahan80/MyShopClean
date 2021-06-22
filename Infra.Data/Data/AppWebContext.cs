using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

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
        public DbSet<Adjective> Adjectives{ get; set; }
        public DbSet<Value> Values { get; set; }
        public DbSet<SubValue> SubValues { get; set; }
        public DbSet<Category> Categories { get; set; }       
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }       
        public DbSet<RoleModel> RoleModels { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }       
        public DbSet<UserFavorite> UserFavorites { get; set; }       
        public DbSet<UserFavoritesDetail> UserFavoritesDetails { get; set; }       
        #endregion
        #region ModelCreating
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        #endregion
    }
}
