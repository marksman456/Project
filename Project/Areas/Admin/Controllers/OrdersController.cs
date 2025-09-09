using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Exceptions;
using Project.Services.Interfaces;
using ProjectData.Data;
using ProjectData.Models; 

namespace Project.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromQuotation(int quotationId, int paymethodId, int salesChannelId)
        {

            try
            {
                var createdOrder = await _orderService.CreateOrderFromQuotationAsync(quotationId, paymethodId, salesChannelId);
                TempData["SuccessMessage"] = $"成功由報價單建立新訂單 {createdOrder.OrderNumber}！";
                return RedirectToAction("Details", "Orders", new { id = createdOrder.OrderID });

            }


            catch (InsufficientStockException ex)
            {
                TempData["ErrorMessage"] = $"建立訂單失敗：{ex.Message}";
                return RedirectToAction("Details", "Quotations", new { id = quotationId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"發生未預期的錯誤，訂單建立失敗: {ex.Message}";
                return RedirectToAction("Details", "Quotations", new { id = quotationId });
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var order = await _orderService.MarkOrderAsPaidAsync(id);

            if (order)
            {
                TempData["SuccessMessage"] = "訂單已成功標記為「已付款」。";
            }


            else
            {
                TempData["ErrorMessage"] = "此訂單已經是找不到或是「已付款」狀態。";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkAsShipped(int id)
        {
            var order = await _orderService.MarkOrderAsShippedAsync(id);



            if (order)
            {

                TempData["SuccessMessage"] = "訂單已成功標記為「已出貨」。";
            }
            else
            {
                TempData["ErrorMessage"] = "此訂單狀態無法標記為已出貨。";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetOrdersForIndexAsync();

            return View(orders);
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderService.GetOrderForDetailsAsync(id.Value);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

    }
}