using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectData.Data;
using Project.Services;
using Project.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<XiangYunDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("XiangYunDbContextConnection")));


builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("XiangYunDbContextConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>().AddEntityFrameworkStores<IdentityDbContext>();


builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddScoped<IQuotationService, QuotationService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IInventoryService, InventoryService>();



builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "userDefault",
    pattern: "",
    defaults: new { area = "User", controller = "Home", action = "Index" }
);

app.MapControllerRoute(
    name: "adminDefault",
    pattern: "admin",
    defaults: new { area = "Admin", controller = "Dashboard", action = "Index" }
);


app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" }
);

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");






app.Run();
