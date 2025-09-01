using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data; // 請替換為你的專案 Data 命名空間
using Project.Models; // 請替換為你的專案 Models 命名空間

namespace Project.Areas.Admin.Controllers // 請替換為你的專案 Area 命名空間
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly XiangYunDbContext _context; // 請替換為你的 DbContext

        public OrdersController(XiangYunDbContext context) // 請替換為你的 DbContext
        {
            _context = context;
        }

        // POST: Admin/Orders/CreateFromQuotation
        // POST: Admin/Orders/CreateFromQuotation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromQuotation(int quotationId, int paymethodId, int salesChannelId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var quotation = await _context.Quotation
                    .Include(q => q.QuotationDetail)
                        .ThenInclude(qd => qd.ProductDetail)
                            .ThenInclude(pd => pd.Product)
                    .FirstOrDefaultAsync(q => q.QuotationID == quotationId);

                // ... (業務邏輯檢查維持不變) ...
                if (quotation == null) { /* ... */ }
                if (quotation.Status != "報價中") { /* ... */ }
                foreach (var detail in quotation.QuotationDetail) { /* ... */ }

                // --- 建立 Order (已包含 Status, Note, TotalAmount) ---
                var order = new Order
                {
                    OrderNumber = await GenerateOrderNumberAsync(),
                    MemberID = quotation.MemberID,
                    EmployeeID = quotation.EmployeeID,
                    OrderDate = DateTime.Now,
                    IsPaid = false,
                    Status = "訂單成立",
                    Note = quotation.Note,
                    TotalAmount = quotation.QuotationDetail.Sum(d => d.Price * d.Quantity * (d.Discount ?? 1.0m)),
                    PaymethodID = paymethodId,
                    SalesChannelID = salesChannelId
                };

                // --- 複製明細 (已包含 Discount) ---
                foreach (var quoteDetail in quotation.QuotationDetail)
                {
                    order.OrderDetail.Add(new OrderDetail
                    {
                        ProductDetailID = quoteDetail.ProductDetailID,
                        Price = quoteDetail.Price,
                        Quantity = quoteDetail.Quantity,
                        Discount = quoteDetail.Discount ?? 1.0m
                    });
                }

                _context.Order.Add(order);

                // --- 更新庫存 (已包含 RelatedOrder) ---
                foreach (var quoteDetail in quotation.QuotationDetail)
                {
                    quoteDetail.ProductDetail.Status = "已售出";

                    _context.InventoryMovement.Add(new InventoryMovement
                    {
                        ProductDetailID = quoteDetail.ProductDetailID,
                        MovementDate = DateTime.Now,
                        MovementType = "銷售出貨",
                        Quantity = -1,
                        // 【最終版】: 建立庫存異動與新訂單的關聯
                        RelatedOrder = order
                    });
                }

                // --- 更新報價單狀態 (維持不變) ---
                quotation.Status = "已成交";
                quotation.IsTransferred = true;

                // --- 儲存所有變更 ---
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"成功由報價單 {quotation.QuotationNumber} 建立新訂單 {order.OrderNumber}！";
                return RedirectToAction("Details", new { id = order.OrderID });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "發生未預期的錯誤，訂單建立失敗：" + ex.Message + (ex.InnerException != null ? " | " + ex.InnerException.Message : "");
                return RedirectToAction("Details", "Quotations", new { id = quotationId });
            }
        }


        public async Task<IActionResult> Index()
        {
            var orders = await _context.Order
                .Include(o => o.Member)        // 載入關聯的客戶資料
                .Include(o => o.Employee)      // 載入關聯的員工資料
                .OrderByDescending(o => o.OrderDate) // 讓最新的訂單顯示在最上面
                .ToListAsync();

            return View(orders);
        }

        // GET: Admin/Orders/Details/5
        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Member)        // 載入客戶
                .Include(o => o.Employee)      // 載入員工
                .Include(o => o.Paymethod) // 載入付款方式
                .Include(o => o.SalesChannel)  // 載入銷售管道
                .Include(o => o.OrderDetail)   // 載入訂單明細
                    .ThenInclude(od => od.ProductDetail) // 根據明細，載入庫存品項
                        .ThenInclude(pd => pd.Product)   // 根據庫存品項，載入產品主檔
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            //O-20231005-001
            var prefix = $"O-{today}-";

            var lastOrder = await _context.Order
                .Where(q => q.OrderNumber.StartsWith(prefix))
                .OrderByDescending(q => q.OrderNumber)
                .FirstOrDefaultAsync();

            int nextSerial = 1;
            if (lastOrder != null)
            {
                var lastSerialStr = lastOrder.OrderNumber.Substring(prefix.Length);
                if (int.TryParse(lastSerialStr, out var lastSerial))
                {
                    nextSerial = lastSerial + 1;
                }
            }

            return $"{prefix}{nextSerial:D3}"; // D4 代表補零到 4 位數，例如 0001
        }
    }
}