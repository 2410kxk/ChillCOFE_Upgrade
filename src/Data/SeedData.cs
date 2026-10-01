using Microsoft.AspNetCore.Identity;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.Data;
public static class SeedData
{
    public static void SeedTables(AppDbContext context)
    {
        if (context.Tables.Any()) return;
        context.Tables.AddRange(Enumerable.Range(1, 6).Select(i => new Table { Name = $"A{i:00}", Status = "Trống" }));
        context.SaveChanges();
    }
    public static async Task CreateDemoAsync(IServiceProvider services, IConfiguration config)
    {
        var users = services.GetRequiredService<UserManager<IdentityUser>>();
        var db = services.GetRequiredService<AppDbContext>();
        foreach (var role in new[] { "Admin", "Staff", "User" })
        {
            var email = config[$"Seed:{role}:Email"];
            var password = config[$"Seed:{role}:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException($"Missing development seed credentials for {role}.");
            var user = await users.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var created = await users.CreateAsync(user, password);
                if (!created.Succeeded) throw new InvalidOperationException(string.Join("; ", created.Errors.Select(e => e.Description)));
            }
            if (!await users.IsInRoleAsync(user, role))
            {
                var assigned = await users.AddToRoleAsync(user, role);
                if (!assigned.Succeeded) throw new InvalidOperationException("Cannot assign demo role.");
            }
        }
        if (!db.Products.Any())
        {
            var category = db.Categories.FirstOrDefault(c => c.Name == "Cà phê");
            if (category == null) { category = new Category { Name = "Cà phê" }; db.Categories.Add(category); }
            db.Products.AddRange(
                new Product { Name = "Cà phê đen", Price = 20000, Category = category, ImageUrl = "", Description = "Dữ liệu demo" },
                new Product { Name = "Cà phê sữa", Price = 25000, Category = category, ImageUrl = "", Description = "Dữ liệu demo" },
                new Product { Name = "Cà phê muối", Price = 35000, Category = category, ImageUrl = "", Description = "Dữ liệu demo" });
            await db.SaveChangesAsync();
        }
    }
}
