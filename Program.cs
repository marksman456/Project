using Microsoft.EntityFrameworkCore;
using Project.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<XiangYunDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("XiangYunDbContextConnection")));




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



// 1. �w�]�i�J User �ϰ쭺���]�X�� "/" �ɡ^
app.MapControllerRoute(
    name: "userDefault",
    pattern: "",
    defaults: new { area = "User", controller = "Home", action = "Index" }
);

// 2. �w�]�i�J Admin �ϰ쭺���]�X�� "/admin" �ɡ^
app.MapControllerRoute(
    name: "adminDefault",
    pattern: "admin",
    defaults: new { area = "Admin", controller = "Members", action = "Index" }
);

// 3. Admin �ϰ줺���M�θ��ѡ]�קK�M User �V�c�^
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" }
);

// 4. �q�� Area ���ѡ]���t�Φ۰ʳB�z��L�ϰ�^
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// 5. �w�]�]�D�ϰ�^���ѡA���S���ϥ� Area �����p
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");






app.Run();
