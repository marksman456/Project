using ProjectData.Models;

namespace Project.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        // KPI 卡片的數據
        public decimal MonthlyRevenue { get; set; }
        public int PendingOrdersCount { get; set; }
        public int QuotesTodayCount { get; set; }
        public int TotalStockItemsCount { get; set; }

        // 列表的數據
        public List<Order> RecentOrders { get; set; } = new();
        public List<ProductDetail> LowStockItems { get; set; } = new();
    }
}