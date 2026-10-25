using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
using QuanLiQuanCaffee.Services;
var dbPath=Environment.GetEnvironmentVariable("CHILL_TEST_DB") ?? "/tmp/chill-online-test.db";
var builder=WebApplication.CreateBuilder(new WebApplicationOptions{Args=args,ApplicationName=typeof(AppDbContext).Assembly.FullName,WebRootPath=Path.GetFullPath("work/ChillCOFE_Upgrade/src/wwwroot")});
builder.Services.AddDbContext<AppDbContext>(o=>o.UseSqlite("Data Source="+dbPath));
builder.Services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.ConfigureApplicationCookie(o=>{o.Cookie.SecurePolicy=CookieSecurePolicy.None;});
builder.Services.AddHttpContextAccessor();builder.Services.AddScoped<SiteData>();builder.Services.AddScoped<ImageUpload>();
builder.Services.AddControllersWithViews().AddApplicationPart(typeof(AppDbContext).Assembly);builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();builder.Services.AddSession();builder.Services.AddAntiforgery(o=>o.HeaderName="RequestVerificationToken");
var app=builder.Build();
using(var scope=app.Services.CreateScope()){
var db=scope.ServiceProvider.GetRequiredService<AppDbContext>();await db.Database.EnsureDeletedAsync();await db.Database.EnsureCreatedAsync();
var roles=scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();var users=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
foreach(var role in new[]{"Admin","Staff","User"})await roles.CreateAsync(new IdentityRole(role));
foreach(var name in new[]{"admin","staff","alice","bob"}){var user=new IdentityUser{UserName=name,Email=name+"@test.invalid",EmailConfirmed=true};var result=await users.CreateAsync(user,"TestOnly!234");if(!result.Succeeded)throw new Exception("seed user failed");await users.AddToRoleAsync(user,name=="admin"?"Admin":name=="staff"?"Staff":"User");}
var cat=new Category{Name="Cà phê"};db.Categories.Add(cat);await db.SaveChangesAsync();db.Products.Add(new Product{Name="Bạc xỉu",CategoryID=cat.CategoryID,Price=35000,Description="Cà phê và sữa",ImageUrl="/images/editorial/coffee-art.png",IsAvailable=true});foreach(var name in new[]{"Cà phê sữa","Cappuccino","Latte","Espresso","Cà phê muối","Trà đào","Cold brew"})db.Products.Add(new Product{Name=name,CategoryID=cat.CategoryID,Price=45000,Description="Dữ liệu kiểm thử",ImageUrl="/images/editorial/coffee-art.png",IsAvailable=true});db.Tables.Add(new Table{Name="Bàn 01",Status="Trống",Note="2 người · gần cửa sổ"});await db.SaveChangesAsync();}
app.UseStaticFiles();app.UseRouting();app.UseSession();app.UseAuthentication();app.UseAuthorization();app.MapControllerRoute("default","{controller=Home}/{action=Index}/{id?}");app.MapRazorPages();app.Run("http://127.0.0.1:5088");
