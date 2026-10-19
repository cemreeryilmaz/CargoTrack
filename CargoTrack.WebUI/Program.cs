using CargoTrack.Business;
using CargoTrack.Business.Services.Abouts;
using CargoTrack.Business.Services.AuditLogs;
using CargoTrack.Business.Services.Branchs;
using CargoTrack.Business.Services.CargoPrices;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.Business.Services.Cities;
using CargoTrack.Business.Services.Dashboards;
using CargoTrack.Business.Services.Employees;
using CargoTrack.Business.Services.Pricing;
using CargoTrack.Business.Services.Reports;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abouts;
using CargoTrack.DataAccess.Repositories.AuditLogs;
using CargoTrack.DataAccess.Repositories.Branchs;
using CargoTrack.DataAccess.Repositories.CargoMovements;
using CargoTrack.DataAccess.Repositories.CargoPrices;
using CargoTrack.DataAccess.Repositories.Cargos;
using CargoTrack.DataAccess.Repositories.Cities;
using CargoTrack.DataAccess.Repositories.Deliveries;
using CargoTrack.DataAccess.Repositories.DeliveryExceptions;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.DataAccess.Repositories.TransferCenters;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using CargoTrack.WebUI.Consts;
using CargoTrack.WebUI.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------- FluentValidation ----------------
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssembly(typeof(BussinessAssembly).Assembly);

// ---------------- Repositories (Repository Design Pattern) ----------------
builder.Services.AddScoped<IAboutRepository, AboutRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();
builder.Services.AddScoped<ICargoMovementRepository, CargoMovementRepository>();
builder.Services.AddScoped<ITransferCenterRepository, TransferCenterRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IDeliveryExceptionRepository, DeliveryExceptionRepository>();
builder.Services.AddScoped<ICargoPriceRepository, CargoPriceRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// ---------------- Services (domain bazlı özel servisler) ----------------
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICargoPricingService, CargoPricingService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<ITransferCenterService, TransferCenterService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICargoPriceService, CargoPriceService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportService, ReportService>();

// ---------------- DbContext ----------------
builder.Services.AddDbContext<CargoTrackDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});

// ---------------- Identity ----------------
builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<CargoTrackDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index";
    options.LogoutPath = "/Login/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.Cookie.Name = "CargoTrackCookie";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ---------------- Pipeline ----------------
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Global Exception Handling
app.UseGlobalExceptionHandling();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}");

// ---------------- Migration + Seed ----------------
await SeedDataAsync(app);

app.Run();


// ================= Seed =================
static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<CargoTrackDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();

    // Bekleyen migration'ları uygula (mevcut veriyi korur).
    await context.Database.MigrateAsync();

    // Şehirler
    if (!await context.Cities.AnyAsync())
    {
        var cityNames = new[]
        {
            "İstanbul", "Ankara", "İzmir", "Bursa", "Antalya", "Adana", "Konya",
            "Şanlıurfa", "Gaziantep", "Kocaeli", "Mersin", "Diyarbakır", "Hatay",
            "Kayseri", "Samsun", "Balıkesir", "Kahramanmaraş", "Van", "Aydın", "Tekirdağ"
        };
        context.Cities.AddRange(cityNames.Select(n => new City { Name = n }));
        await context.SaveChangesAsync();
    }

    // Roller (RoleManager ile NormalizedName doğru set edilir)
    foreach (var roleName in new[] { Roles.Admin, Roles.Manager, Roles.User })
    {
        if (!await roleManager.RoleExistsAsync(roleName))
            await roleManager.CreateAsync(new AppRole { Name = roleName });
    }

    // Varsayılan fiyat kuralları
    if (!await context.CargoPrices.AnyAsync())
    {
        context.CargoPrices.AddRange(
            new CargoPrice { Name = "Standart Tarife", DeliveryType = DeliveryType.Standard, BasePrice = 50m, PricePerKg = 10m, PricePerDesi = 8m, DifferentCityFee = 25m, IsActive = true },
            new CargoPrice { Name = "Ekspres Tarife", DeliveryType = DeliveryType.Express, BasePrice = 90m, PricePerKg = 14m, PricePerDesi = 11m, DifferentCityFee = 35m, IsActive = true },
            new CargoPrice { Name = "Aynı Gün Tarife", DeliveryType = DeliveryType.SameDay, BasePrice = 150m, PricePerKg = 20m, PricePerDesi = 16m, DifferentCityFee = 50m, IsActive = true }
        );
        await context.SaveChangesAsync();
    }

    // Varsayılan admin kullanıcısı
    const string adminEmail = "admin@cargotrack.com";
    if (await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new AppUser
        {
            UserName = "admin",
            Email = adminEmail,
            FirstName = "Sistem",
            LastName = "Yöneticisi",
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, Roles.Admin);
    }
}
