
using Microsoft.EntityFrameworkCore;
using Project.Services.Interfaces;
using Project.ViewModels.VMDashboard;
using ProjectData.Data;
using ProjectData.Models;

namespace Project.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly XiangYunDbContext _context;

        public DashboardService(XiangYunDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardViewModelAsync()
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
            var lowStockGroups = await _context.ProductDetail
            .Include(pd => pd.Product)
                .ThenInclude(p => p.ProductModel)
                    .ThenInclude(pm => pm.ModelSpec)
            .Where(pd => !pd.Product.IsSerialized && pd.Status == "庫存中")
            .GroupBy(pd => new
            {
                pd.ProductID,
                pd.Product.ProductModelID,
                pd.PurchaseCost
            })
            .Select(g => new
            {
                ProductID = g.Key.ProductID,
                ProductModelID = g.Key.ProductModelID,
                PurchaseCost = g.Key.PurchaseCost,
                TotalQuantity = g.Sum(x => x.Quantity),
                // 取一筆代表性 ProductDetail（可用於顯示商品資訊與關聯）
                SampleDetail = g.OrderBy(x => x.ProductDetailID).FirstOrDefault()
            })
            .Where(x => x.TotalQuantity < 10)
            .OrderBy(x => x.TotalQuantity)
            .Take(5)
            .ToListAsync();

            // 轉成 List<ProductDetail> 以相容原本 ViewModel
            var lowStockItems = lowStockGroups
                .Select(x =>
                {
                    var detail = x.SampleDetail;
                    if (detail != null)
                    {
                        detail.Quantity = x.TotalQuantity; // 顯示加總後的數量
                    }
                    return detail;
                })
                .OfType<ProductDetail>() // 過濾 null 並正確型別
                .ToList();

            return new DashboardViewModel
            {
                MonthlyRevenue = monthlyRevenue,
                PendingOrdersCount = pendingOrdersCount,
                QuotesTodayCount = quotesTodayCount,
                TotalStockItemsCount = totalStockItemsCount,
                RecentOrders = recentOrders,
                LowStockItems = lowStockItems
            };
        }
    }
}
