using Application.InterFaces.Admin;
using Application.InterFaces.Both;
using Application.InterFaces.User;
using Application.Security.AuthenTication;
using Application.Services.Admin;
using Application.Services.Both;
using Application.Services.User;
using Data.Repositories.AdminRepositories;
using Domain.InterFaces;
using Domain.InterFaces.AdminInterFaces;
using Domain.Models;
using Infra.Data;
using Infra.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Infra.Ioc.Dependencies
{
    public static class DependencyContainer
    {
        public static void Registerservice(IServiceCollection services)
        {

            //Admin InterFaces And Repositories

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryServices, CategoryServices>();
            services.AddScoped<ICategoryUserServices, CategoryUserServices>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProudctServices, ProductServices>();
            services.AddScoped<IProductUserServices, ProductUserServices>();

            services.AddScoped<IMessageSenderServices, MessageSenderServices>();

            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<IAccountUserServices, AccountUserServices>();


            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<ICommentUserServices, CommentUserServices>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IQuestionRepository, QuestionRepository>();


            services.AddScoped<IBanerRepository, BanerRepository>();
            services.AddScoped<IBanerServices, BanerServices>();


            services.AddScoped<IHomePageServices, HomePageServices>();

            services.AddScoped<IPayRepository, PayRepository>();
            services.AddScoped<IPayUserServices, PayUserServices>();

            services.AddScoped<IContactUsRepository,ContactUsRepository>();




            //Authorization

            services.AddAuthorizationCore(option =>
            {
                option.AddPolicy("Founder",
                    policy => policy.Requirements.Add(new PolicyRequirenment("Founder")));

                option.AddPolicy("Manager",
                    policy => policy.Requirements.Add(new PolicyRequirenment("Manager")));

                option.AddPolicy("Writer",
                    policy => policy.Requirements.Add(new PolicyRequirenment("Writer")));

                option.AddPolicy("Customer",
                    policy => policy.Requirements.Add(new PolicyRequirenment("Customer")));
            });

            services.AddIdentity<ApplicationUser, RoleModel>(option =>
            {
                option.Password.RequiredLength = 8;
                option.Password.RequireDigit = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequiredUniqueChars = 0;
                option.User.RequireUniqueEmail = true;
                option.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
                option.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppWebContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(option =>
            {
                option.AccessDeniedPath = "/Account/AccessDenied";
                option.LoginPath = "/Account/Login";
                option.LogoutPath = "/Account/Logout";
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                options.ValidationInterval = TimeSpan.Zero;
            });

        }

    }
}
