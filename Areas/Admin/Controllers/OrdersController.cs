using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectData.Data;
using ProjectData.Models; 

namespace Project.Areas.Admin.Controllers { 
    [Area("Admin")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly XiangYunDbContext _context; 

        public OrdersController(XiangYunDbContext context) 
        {
            _context = context;
        }

       
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

                if (quotation == null || quotation.Status != "報價中")
                {
                    TempData["ErrorMessage"] = "報價單不存在或狀態不符，無法建立訂單。";
                    return RedirectToAction("Index", "Quotations");
                }

                // --- 【核心修改開始】 ---
                // 步驟 1: 預先檢查所有品項的庫存
                foreach (var item in quotation.QuotationDetail)
                {
                    if (item.ProductDetail.Product.IsSerialized)
                    {
                        // 檢查序號化商品是否仍為「庫存中」
                        if (item.ProductDetail.Status != "庫存中")
                        {
                            await transaction.RollbackAsync();
                            TempData["ErrorMessage"] = $"建立訂單失敗：商品 '{item.ProductDetail.Product.ProductName}' (序號: {item.ProductDetail.SerialNumber}) 已被售出或狀態異常。";
                            return RedirectToAction("Details", "Quotations", new { id = quotationId });
                        }
                    }
                    else
                    {
                        // 檢查批量商品數量是否足夠
                        if (item.ProductDetail.Quantity < item.Quantity)
                        {
                            await transaction.RollbackAsync();
                            TempData["ErrorMessage"] = $"建立訂單失敗：商品 '{item.ProductDetail.Product.ProductName}' 庫存不足 (庫存: {item.ProductDetail.Quantity}, 訂購: {item.Quantity})。";
                            return RedirectToAction("Details", "Quotations", new { id = quotationId });
                        }
                    }
                }

                // 步驟 2: 建立訂單主檔
                var order = new Order {

                    OrderNumber = await GenerateOrderNumberAsync(), // 產生訂單編號
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
                _context.Order.Add(order);

                // 步驟 3: 處理訂單明細與庫存扣除
                foreach (var quoteDetail in quotation.QuotationDetail)
                {
                    // 複製報價明細到訂單明細
                    order.OrderDetail.Add(new OrderDetail
                    {
                        ProductDetailID = quoteDetail.ProductDetailID,
                        Price = quoteDetail.Price,
                        Quantity = quoteDetail.Quantity,
                        Discount = quoteDetail.Discount
                    });

                    // 根據商品類型，執行不同的庫存扣除邏輯
                    var productDetailToUpdate = quoteDetail.ProductDetail;
                    if (productDetailToUpdate.Product.IsSerialized)
                    {
                        // 序號化商品：更新狀態
                        productDetailToUpdate.Status = "已售出";
                        productDetailToUpdate.Quantity -= quoteDetail.Quantity;
                    }
                    else
                    {
                        // 批量商品：扣減數量
                        productDetailToUpdate.Quantity -= quoteDetail.Quantity;
                    }
                    _context.Update(productDetailToUpdate);

                    // 新增庫存異動紀錄
                    _context.InventoryMovement.Add(new InventoryMovement
                    {
                        ProductDetail = productDetailToUpdate,
                        MovementDate = DateTime.Now,
                        MovementType = "銷售出貨",
                        Quantity = -quoteDetail.Quantity, // 出庫，數量為負
                        RelatedOrder = order
                    });
                }
                // --- 【核心修改結束】 ---

                // 更新原始報價單狀態
                quotation.Status = "已成交";
                quotation.IsTransferred = true;
                _context.Update(quotation);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"成功由報價單 {quotation.QuotationNumber} 建立新訂單 {order.OrderNumber}！";
                return RedirectToAction("Details", "Orders", new { id = order.OrderID });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // --- 【新的偵錯程式碼】 ---
                // 建立一個詳細的錯誤訊息字串
                string errorMessage = ex.Message;

                // 檢查是否有內部例外 (Inner Exception)
                if (ex.InnerException != null)
                {
                    // 如果有，就將它的訊息也加進來
                    errorMessage += " |---> Inner Exception: " + ex.InnerException.Message;
                }

                // 將完整的錯誤訊息，透過 TempData 傳到下一個頁面
                TempData["ErrorMessage"] = $"發生未預期的錯誤，訂單建立失敗: {errorMessage}";
                // --- 【偵錯程式碼結束】 ---

                return RedirectToAction("Details", "Quotations", new { id = quotationId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.IsPaid)
            {
                TempData["ErrorMessage"] = "此訂單已經是「已付款」狀態。";
            }
            else
            {
                order.IsPaid = true;
                // (可選) 同時更新訂單狀態，讓流程更清晰
                order.Status = "待出貨";

                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"訂單 {order.OrderNumber} 已成功標記為「已付款」。";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsShipped(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // 只有在「待出貨」狀態才能標記為「已出貨」
            if (order.Status == "待出貨")
            {
                order.Status = "已出貨";
                // (可選) 我們可以新增一個「出貨日期」的欄位來記錄
                // order.ShippedDate = DateTime.Now; 

                _context.Update(order);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"訂單 {order.OrderNumber} 已成功標記為「已出貨」。";
            }
            else
            {
                TempData["ErrorMessage"] = $"此訂單狀態為「{order.Status}」，無法標記為已出貨。";
            }

            return RedirectToAction(nameof(Details), new { id = id });
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