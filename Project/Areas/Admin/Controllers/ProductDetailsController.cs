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

        public async Task<IActionResult> Index()
        {
            var viewModel = await _inventoryService.GetInventoryForIndexAsync();
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
                catch (Exception)
                {
                    // 捕捉其他未預期錯誤
                    ModelState.AddModelError(string.Empty, "發生未預期的錯誤，請稍後再試。");
                }
            }
            var repopulatedViewModel = await _inventoryService.PrepareNewStockInViewModelAsync();
            viewModel.ProductList = repopulatedViewModel.ProductList;
            return View(viewModel);
        }
    }
      
}

