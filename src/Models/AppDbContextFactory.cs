using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuanLiQuanCaffee.Models
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true).AddUserSecrets<AppDbContextFactory>()
                .AddEnvironmentVariables().Build();
            var connection = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connection)) throw new InvalidOperationException("Chưa có ConnectionStrings:DefaultConnection. Cấu hình User Secrets trước khi dùng EF.");
            optionsBuilder.UseSqlServer(connection);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
