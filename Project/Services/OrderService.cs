using Microsoft.EntityFrameworkCore;
using Project.Exceptions;
using Project.Services.Interfaces;
using Project.ViewModels.VMOrder;
using ProjectData.Data;
using ProjectData.Models;

namespace Project.Services
{
    public class OrderService : IOrderService
    {
        private readonly XiangYunDbContext _context;

        public OrderService(XiangYunDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderIndexViewModel>> GetOrdersForIndexAsync()
        {
            return await _context.Order
                .Include(o => o.Member)
                .Include(o => o.Employee)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderIndexViewModel
                {
                    OrderID = o.OrderID,
                    OrderNumber = o.OrderNumber,
                    MemberName = o.Member.Name,
                    EmployeeName = o.Employee.Name,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    IsPaid = o.IsPaid,
                    Status = o.Status
                }).ToListAsync();
        }

        public async Task<Order?> GetOrderForDetailsAsync(int id)
        {
            var order = await _context.Order
                     .Include(o => o.Member)        // 載入客戶
                     .Include(o => o.Employee)      // 載入員工
                     .Include(o => o.Paymethod) // 載入付款方式
                     .Include(o => o.SalesChannel)  // 載入銷售管道
                     .Include(o => o.OrderDetail)   // 載入訂單明細
                         .ThenInclude(od => od.ProductDetail) // 根據明細，載入庫存品項
                             .ThenInclude(pd => pd.Product)   // 根據庫存品項，載入產品主檔
                     .FirstOrDefaultAsync(m => m.OrderID == id);

            return order;
        }

        public async Task<Order> CreateOrderFromQuotationAsync(int quotationId, int paymethodId, int salesChannelId)
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
                    throw new InvalidOperationException("報價單不存在或狀態不符，無法建立訂單。");
                }

                // 預先檢查所有品項的庫存
                foreach (var item in quotation.QuotationDetail)
                {
                    if (item.ProductDetail.Product.IsSerialized)
                    {
                        if (item.ProductDetail.Status != "庫存中")
                        {
                            throw new InsufficientStockException($"商品 '{item.ProductDetail.Product.ProductName}' (序號: {item.ProductDetail.SerialNumber}) 已被售出或狀態異常。");
                        }
                    }
                    else
                    {
                        if (item.ProductDetail.Quantity < item.Quantity)
                        {
                            throw new InsufficientStockException($"商品 '{item.ProductDetail.Product.ProductName}' 庫存不足 (庫存: {item.ProductDetail.Quantity}, 訂購: {item.Quantity})。");
                        }
                    }
                }

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
                    SalesChannelID = salesChannelId,
                    OrderDetail = new List<OrderDetail>()
                };
                _context.Order.Add(order);

                // 處理訂單明細與庫存扣除
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

                quotation.Status = "已成交";
                quotation.IsTransferred = true;
                _context.Update(quotation);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // Re-throw the exception to be handled by the controller
            }
        }

        public async Task<bool> MarkOrderAsPaidAsync(int id) 
        {
            var order = await _context.Order.FindAsync(id);

            if (order == null || (order.Status != "訂單成立" && order.Status != "已出貨"))
            {
                return false;
            }

            if (order.IsPaid)
            {
                return true;
            }

            order.IsPaid = true;
            
            _context.Update(order);
            await _context.SaveChangesAsync();


            return true; 
        }

        public async Task<bool> MarkOrderAsShippedAsync(int id) 
        {

            var order = await _context.Order.FindAsync(id);

            if (order == null || (order.Status != "待出貨" && order.Status != "訂單成立"))
            {
                return false; // 訂單不存在或狀態不符，操作失敗
            }


            order.Status = "已出貨";
            _context.Update(order);
            await _context.SaveChangesAsync();

            return true; // 操作成功

            
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

            return $"{prefix}{nextSerial:D3}"; // D3 代表補零到 3 位數，例如 001
        }
    }
}