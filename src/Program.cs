using System.Diagnostics;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuanLiQuanCaffee.Data;
using QuanLiQuanCaffee.Models;

var builder = WebApplication.CreateBuilder(args);

// ==================== 1. DỊCH VỤ CHUNG ====================

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    QuanLiQuanCaffee.Services.SiteData>();

builder.Services.AddScoped<
    QuanLiQuanCaffee.Services.ImageUpload>();

// ==================== 2. DATABASE ====================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Chưa cấu hình ConnectionStrings:DefaultConnection. " +
        "Chạy với môi trường Development và thiết lập User Secrets " +
        "theo README_NANG_CAP.md.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ==================== 3. IDENTITY VÀ PHÂN QUYỀN ====================

builder.Services
    .AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api") ||
            context.Request.Path.StartsWithSegments("/dev/antiforgery"))
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }

        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api") ||
            context.Request.Path.StartsWithSegments("/dev/antiforgery"))
        {
            context.Response.StatusCode =
                StatusCodes.Status403Forbidden;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }

        return Task.CompletedTask;
    };

    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// ==================== 4. SESSION ====================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ==================== 5. MVC VÀ ANTIFORGERY ====================

builder.Services
    .AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        options.ViewLocationExpanders.Add(
            new GlobalViewDataExpander());
    });

builder.Services.AddRazorPages();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

// ==================== 6. SWAGGER ====================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Phân biệt các lớp Input trong những controller khác nhau.
    options.CustomSchemaIds(type =>
        (type.FullName ?? type.Name).Replace("+", "."));

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Chill COFÉ API",
        Version = "v1",
        Description = "Kiểm thử API bán hàng và thanh toán."
    });

    options.DocInclusionPredicate((documentName, api) =>
        documentName == "v1" &&
        new[]
        {
            "pos",
            "catalog",
            "tables",
            "customers",
            "statistics",
            "diagnostics"
        }.Contains(api.GroupName));
});

// ==================== 7. KHỞI TẠO ỨNG DỤNG ====================

var app = builder.Build();

// ==================== 8. KHỞI TẠO DỮ LIỆU ====================

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<AppDbContext>();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Staff", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(
                new IdentityRole(role));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Không thể tạo vai trò {role}: {errors}");
            }
        }
    }

    if (app.Environment.IsDevelopment() &&
        builder.Configuration.GetValue<bool>("Seed:Enabled"))
    {
        await SeedData.CreateDemoAsync(
            services,
            builder.Configuration);
    }

    SeedData.SeedTables(context);
}

// ==================== 9. XỬ LÝ EXCEPTION TẬP TRUNG ====================

if (app.Environment.IsDevelopment())
{
    // Trả thông báo an toàn để kiểm thử QLBH-137.
    // Chi tiết exception chỉ ghi trong log server.
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var feature =
                context.Features.Get<IExceptionHandlerFeature>();

            var traceId =
                Activity.Current?.Id ?? context.TraceIdentifier;

            app.Logger.LogError(
                feature?.Error,
                "Lỗi request. Path={Path}; TraceId={TraceId}",
                context.Request.Path,
                traceId);

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            context.Response.Headers["Cache-Control"] = "no-store";

            await context.Response.WriteAsJsonAsync(new
            {
                status = StatusCodes.Status500InternalServerError,
                message = "Đã xảy ra lỗi. Vui lòng thử lại sau.",
                traceId
            });
        });
    });
}
else
{
    // Giữ trang xử lý lỗi của website ngoài Development.
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ==================== 10. MIDDLEWARE ====================

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Giữ quy tắc ngừng phục vụ các đường dẫn đặt món online cũ.
app.Use(async (context, next) =>
{
    var values = context.Request.RouteValues;

    var controller =
        Convert.ToString(values["controller"]) ?? "";

    var action =
        Convert.ToString(values["action"]) ?? "";

    var onlinePage =
        (controller.Equals(
             "Product",
             StringComparison.OrdinalIgnoreCase) &&
         action.Equals(
             "Menu",
             StringComparison.OrdinalIgnoreCase)) ||
        (controller.Equals(
             "Order",
             StringComparison.OrdinalIgnoreCase) &&
         (action.Equals(
              "OnlineCart",
              StringComparison.OrdinalIgnoreCase) ||
          action.Equals(
              "OnlineCheckout",
              StringComparison.OrdinalIgnoreCase)));

    var onlineSubmit =
        controller.Equals(
            "Order",
            StringComparison.OrdinalIgnoreCase) &&
        (action.Equals(
             "PlaceOnlineOrder",
             StringComparison.OrdinalIgnoreCase) ||
         action.Equals(
             "AddToOnlineCart",
             StringComparison.OrdinalIgnoreCase));

    if (onlinePage || onlineSubmit)
    {
        if (onlinePage &&
            (HttpMethods.IsGet(context.Request.Method) ||
             HttpMethods.IsHead(context.Request.Method)))
        {
            context.Response.Redirect("/Cafe/Menu");
        }
        else
        {
            context.Response.StatusCode =
                StatusCodes.Status410Gone;

            await context.Response.WriteAsJsonAsync(new
            {
                message =
                    "Đặt món online đã ngừng phục vụ. " +
                    "Vui lòng đặt tại quầy."
            });
        }

        return;
    }

    await next(context);
});

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

// ==================== 11. CÔNG CỤ TRONG DEVELOPMENT ====================

if (app.Environment.IsDevelopment())
{
    // Cấp antiforgery token cho phiên Admin/Staff hiện tại.
    app.MapGet(
            "/dev/antiforgery",
            (HttpContext context, IAntiforgery antiforgery) =>
            {
                context.Response.Headers["Cache-Control"] = "no-store";

                var tokens =
                    antiforgery.GetAndStoreTokens(context);

                return Results.Ok(new
                {
                    requestToken = tokens.RequestToken
                });
            })
        .RequireAuthorization(policy =>
            policy.RequireRole("Admin", "Staff"))
        .ExcludeFromDescription();

    // Endpoint kiểm thử exception:
    // - Chỉ xuất hiện trong Development.
    // - Chỉ bật khi có cấu hình EnableExceptionTest=true.
    // - Chỉ Admin/Staff được gọi.
    if (builder.Configuration.GetValue<bool>(
            "DeveloperTools:EnableExceptionTest"))
    {
        static IResult ThrowControlledException()
        {
            throw new InvalidOperationException(
                "QLBH-137: Loi kiem thu co kiem soat.");
        }

        app.MapGet(
                "/api/dev/exception-test",
                () => ThrowControlledException())
            .WithGroupName("diagnostics")
            .WithSummary(
                "QLBH-137 - Kiểm thử xử lý exception")
            .Produces(
                StatusCodes.Status500InternalServerError)
            .RequireAuthorization(policy =>
                policy.RequireRole("Admin", "Staff"));
    }

    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Chill COFÉ API v1");

        options.RoutePrefix = "swagger";

        options.DocumentTitle =
            "Chill COFÉ - Swagger UI";

        options.InjectJavascript(
            "/js/swagger-auth.js");

        options.UseRequestInterceptor(
            "function(request) { " +
            "return window.chillSwaggerRequest(request); " +
            "}");
    });
}

// ==================== 12. ROUTING ====================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();