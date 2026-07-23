using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infra.Data
{
    public class AppWebContextFactory : IDesignTimeDbContextFactory<AppWebContext>
    {
        public AppWebContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppWebContext>()
                .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyShopDesignTime;Trusted_Connection=True;TrustServerCertificate=True")
                .Options;

            return new AppWebContext(options);
        }
    }
}
