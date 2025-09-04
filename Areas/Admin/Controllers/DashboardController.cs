using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using Project.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Areas.Admin.Controllers // 請確認您的命名空間
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly XiangYunDbContext _context;

        public DashboardController(XiangYunDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // 1. KPI 卡片數據查詢
            decimal monthlyRevenue = await _context.Order
                .Where(o => o.OrderDate >= firstDayOfMonth && o.OrderDate <= lastDayOfMonth)
                .SumAsync(o => o.TotalAmount);

            int pendingOrdersCount = await _context.Order
                .CountAsync(o => o.Status == "待出貨");

            int quotesTodayCount = await _context.Quotation
                .CountAsync(q => q.CreatedDate.Year == today.Year && q.CreatedDate.Month == today.Month && q.CreatedDate.Day == today.Day);

            // 計算庫存總數 (序號化 + 批量)
            int serializedStockCount = await _context.ProductDetail
                .Include(pd => pd.Product)
                .Where(pd => pd.Product.IsSerialized && pd.Status == "庫存中")
                .CountAsync();
            int nonSerializedStockSum = await _context.ProductDetail
                .Include(pd => pd.Product)
                .Where(pd => !pd.Product.IsSerialized && pd.Status == "庫存中")
                .SumAsync(pd => pd.Quantity);
            int totalStockItemsCount = serializedStockCount + nonSerializedStockSum;

            // 2. 最新訂單列表
            var recentOrders = await _context.Order
                .Include(o => o.Member)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToListAsync();

            // 3. 低庫存警示 (假設低於 10 件為低庫存)
            var lowStockItems = await _context.ProductDetail
                .Include(pd => pd.Product)
                .Where(pd => !pd.Product.IsSerialized && pd.Quantity < 10 && pd.Status == "庫存中")
                .OrderBy(pd => pd.Quantity)
                .Take(5)
                .ToListAsync();

            // 組合 ViewModel
            var viewModel = new DashboardViewModel
            {
                MonthlyRevenue = monthlyRevenue,
                PendingOrdersCount = pendingOrdersCount,
                QuotesTodayCount = quotesTodayCount,
                TotalStockItemsCount = totalStockItemsCount,
                RecentOrders = recentOrders,
                LowStockItems = lowStockItems
            };

            return View(viewModel);
        }
    }
}