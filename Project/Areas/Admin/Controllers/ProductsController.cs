using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Models.DTOs;
using Project.Services;
using Project.Services.Interfaces;
using Project.ViewModels.VMProduct;
using ProjectData.Data;
using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace Project.Areas.Admin.Controllers 
{

    [Area("Admin")]
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;



        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

       
        public async Task<IActionResult> Index()
        {

            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {

            var viewModel = await _productService.PrepareNewProductViewModelAsync();
            return View(viewModel);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _productService.CreateProductAsync(viewModel);
                TempData["SuccessMessage"] = "產品新增成功！";
                return RedirectToAction(nameof(Index));
            }

            viewModel.ProductModelList = (await _productService.PrepareNewProductViewModelAsync()).ProductModelList;
            return View(viewModel);
        }
            
          

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();


            var viewModel = await _productService.PrepareEditProductViewModelAsync(id.Value);


            if (viewModel == null) return NotFound();

           


            return View(viewModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel viewModel)
        {
            if (id != viewModel.ProductID) return NotFound();

            if (ModelState.IsValid)
            {

                    try
                    {
                        await _productService.UpdateProductAsync(viewModel);
                        TempData["SuccessMessage"] = "產品更新成功！";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (System.Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, $"更新失敗：{ex.Message}");
                    }
            
            }
            viewModel.ProductModelList = (await _productService.PrepareEditProductViewModelAsync(id)).ProductModelList;
            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null) return NotFound();
            return View(product);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            TempData["SuccessMessage"] = "產品刪除成功！";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public async Task<IActionResult> GetProductInfo(int id)
        {
            var productInfo = await _productService.GetProductByIdAsync(id);
            if (productInfo == null)
            {
                return NotFound();
            }
            return Json(new { isSerialized = productInfo.IsSerialized });
        }

        [HttpGet]
      
        public async Task<IActionResult> SearchProducts(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(new List<ProductSearchDTO>());
            }

            var products = await _productService.SearchAvailableProductsAsync(keyword);

            return Json(products);
        }


        [HttpGet]
        public async Task<IActionResult> GetSpecsForModel(int modelId)
        {
            // Controller 的職責：呼叫 Service，並將結果轉成 Json
            var specs = await _productService.GetGroupedSpecsByModelIdAsync(modelId);
            return Json(specs);
        }

    }

}