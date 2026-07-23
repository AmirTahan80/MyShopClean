using Domain.Models;
using Domain.Models.IndexFolder;
using Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyShop.Infrastructure
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppWebContext>();
            if (configuration.GetValue("Database:ApplyMigrationsOnStartup", false))
            {
                await MigrateWithRetryAsync(context);
            }
            else if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                throw new InvalidOperationException(
                    "Pending database migrations were found. Run the deployment migration step or set Database:ApplyMigrationsOnStartup=true for local development.");
            }

            await SeedRolesAsync(scope.ServiceProvider.GetRequiredService<RoleManager<RoleModel>>());
            await SeedCatalogAsync(
                context,
                configuration.GetValue("SeedDemoData:Enabled", false));
            await SeedAdminAsync(
                scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
                configuration);
        }

        private static async Task MigrateWithRetryAsync(AppWebContext context)
        {
            const int attempts = 30;
            for (var attempt = 1; attempt <= attempts; attempt++)
            {
                try
                {
                    await context.Database.MigrateAsync();
                    return;
                }
                catch when (attempt < attempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
        }

        private static async Task SeedRolesAsync(RoleManager<RoleModel> roleManager)
        {
            var roles = new[]
            {
                (Name: "Founder", PersianName: "مدیر کل"),
                (Name: "Manager", PersianName: "مدیر"),
                (Name: "Writer", PersianName: "نویسنده"),
                (Name: "Customer", PersianName: "مشتری")
            };

            foreach (var role in roles)
            {
                if (await roleManager.RoleExistsAsync(role.Name))
                {
                    continue;
                }

                var result = await roleManager.CreateAsync(new RoleModel
                {
                    Name = role.Name,
                    RoleNamePersian = role.PersianName
                });

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
                }
            }
        }

        private static async Task SeedCatalogAsync(AppWebContext context, bool seedDemoData)
        {
            if (!await context.SiteSettings.AnyAsync())
            {
                context.SiteSettings.Add(new SiteSetting
                {
                    SiteName = "MyShop",
                    SiteTagline = "فروشگاه اینترنتی",
                    PrimaryColor = "#EF394E",
                    SecondaryColor = "#0EA5E9",
                    AccentColor = "#F59E0B",
                    FooterTitle = "فروشگاه اینترنتی MyShop",
                    FooterDescription = "خرید آنلاین ساده، امن و سریع با امکان مدیریت کامل محصولات و سفارش‌ها.",
                    FooterCopyright = "تمامی حقوق برای فروشگاه MyShop محفوظ است.",
                    Address = "آدرس فروشگاه را از پنل مدیریت وارد کنید.",
                    Phone = "",
                    SupportEmail = "",
                    PublicBaseUrl = "http://localhost:8080",
                    TorobEnabled = false,
                    EmallsEnabled = false
                });
            }

            if (seedDemoData && !await context.Products.AnyAsync())
            {
                context.Products.AddRange(
                    CreateProduct("گوشی هوشمند نمونه", 24_900_000, 12),
                    CreateProduct("هدفون بی‌سیم نمونه", 2_450_000, 30),
                    CreateProduct("ساعت هوشمند نمونه", 4_800_000, 18));
            }

            if (seedDemoData && !await context.Baners.AnyAsync())
            {
                context.Baners.AddRange(
                    CreateBanner("پیشنهاد ویژه", "Right"),
                    CreateBanner("فروشگاه آنلاین MyShop", "Slider"),
                    CreateBanner("جدیدترین محصولات", "MiddleLeft"),
                    CreateBanner("خرید آسان و سریع", "MiddleRight"));
            }

            await context.SaveChangesAsync();
        }

        private static Product CreateProduct(string name, int price, int count) => new Product
        {
            Name = name,
            Detail = $"توضیحات محصول نمونه: {name}",
            Price = price,
            Count = count,
            InsertTime = DateTime.UtcNow,
            IsProductHaveAttributes = false,
            ProductImages = new[]
            {
                new ProductImages { ImgFile = string.Empty, ImgSrc = "placeholder.svg" }
            }
        };

        private static Baner CreateBanner(string text, string place) => new Baner
        {
            Text = text,
            Link = "/Product",
            Image = "placeholder.svg",
            BanerPlace = place
        };

        private static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var password = configuration["SeedAdmin:Password"];
            if (string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var email = configuration["SeedAdmin:Email"] ?? "admin@myshop.local";
            var userName = configuration["SeedAdmin:UserName"] ?? "admin";
            var admin = await userManager.FindByNameAsync(userName);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
                    RegisterTime = DateTime.UtcNow,
                    UserDetail = new UserDetail()
                };

                var result = await userManager.CreateAsync(admin, password);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Founder"))
            {
                await userManager.AddToRoleAsync(admin, "Founder");
            }
        }
    }
}
