using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Services.Interfaces;
using Project.ViewModels.VMProductDetail;
using ProjectData.Data;
using ProjectData.Models;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductDetailsController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public ProductDetailsController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: Admin/ProductDetails

        public async Task<IActionResult> Index(string? keyword, string? status)
        {
            // 將接收到的查詢條件，傳遞給 Service 層進行處理
          
            var viewModel = await _inventoryService.GetInventoryForIndexAsync(keyword, status);

            // 將查詢條件存入 ViewData，以便 View 可以讀取並保留使用者輸入的值
            ViewData["CurrentKeyword"] = keyword;
            ViewData["CurrentStatus"] = status ?? "All"; // 如果 status 是 null (第一次載入)，預設為 "All"

            return View(viewModel);
        }





        public async Task<IActionResult> Create()
        {
            var viewModel = await _inventoryService.PrepareNewStockInViewModelAsync();
            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDetailCreateViewModel viewModel)
        {

            if (ModelState.IsValid)
            {

                try
                {
                    await _inventoryService.AddStockAsync(viewModel);
                    TempData["SuccessMessage"] = "庫存品項新增成功！";
                    return RedirectToAction(nameof(Index));

                }

                catch (InvalidOperationException ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
                catch (Exception ex) // 加上 ex 變數來接收例外物件
                {
                    // 為了偵錯，最詳細的內部錯誤訊息也顯示出來
                    string errorMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorMessage += " ---> " + ex.InnerException.Message;
                    }
                    ModelState.AddModelError(string.Empty, $"資料庫儲存失敗: {errorMessage}");
                }
            }
            var repopulatedViewModel = await _inventoryService.PrepareNewStockInViewModelAsync();
            viewModel.ProductList = repopulatedViewModel.ProductList;
            return View(viewModel);
        }
    }
      
}

