using Infra.Ioc.Dependencies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Application.InterFaces.Admin;
using Application.Services.Admin;
using Data.Repositories.AdminRepositories;
using Domain.InterFaces.AdminInterFaces;
using Application.Security.AuthenTication;
using Application.InterFaces.Both;
using Application.Services.Both;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Application.Services.User;
using Application.InterFaces.User;

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

        }

    }
}
