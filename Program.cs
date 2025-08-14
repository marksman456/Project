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



// 1. 預設進入 User 區域首頁（訪問 "/" 時）
app.MapControllerRoute(
    name: "userDefault",
    pattern: "",
    defaults: new { area = "User", controller = "Home", action = "Index" }
);

// 2. 預設進入 Admin 區域首頁（訪問 "/admin" 時）
app.MapControllerRoute(
    name: "adminDefault",
    pattern: "admin",
    defaults: new { area = "Admin", controller = "Members", action = "Index" }
);

// 3. Admin 區域內的專用路由（避免和 User 混淆）
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" }
);

// 4. 通用 Area 路由（讓系統自動處理其他區域）
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// 5. 預設（非區域）路由，給沒有使用 Area 的情況
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");






app.Run();
