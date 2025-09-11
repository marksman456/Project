using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using ProjectData.Models;
using Project.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProjectData.Data;



namespace Project.Areas.Admin.Controllers 
{

    [Area("Admin")]
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly XiangYunDbContext _context;
       
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(XiangYunDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment; 
        }

       
        public async Task<IActionResult> Index()
        {
            
            var xiangYunDbContext = _context.Product.Include(p => p.ProductModel);
            return View(await xiangYunDbContext.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            
            var product = await _context.Product
                .Include(p => p.ProductModel)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            
            ViewData["ProductModelId"] = new SelectList(_context.ProductModel, "ProductModelID", "ProductModelName");
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductSKU,ProductName,Description,ProductModelID,Price,IsSerialized")] Product product, IFormFile? formFile)
        {
            if (!ModelState.IsValid)
            {
                 
                   

                
                if (formFile != null && formFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Photos", "products");
                    string uniqueFileName = product.ProductSKU + Path.GetExtension(formFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(fileStream);
                    }
                    
                    product.ProductImage =  uniqueFileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["ProductModelId"] = new SelectList(_context.ProductModel, "ProductModelID", "ProductModelName", product.ProductModelID);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

           
            var product = await _context.Product
                                        .Include(p => p.ProductModel)
                                        .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null) return NotFound();

           
            ViewData["ProductModelID"] = new SelectList(_context.ProductModel, "ProductModelID", "ProductModelName", product.ProductModelID);
            return View(product);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductSKU,ProductName,Description,ProductModelID,Price,ProductImage")] Product product, IFormFile? formFile)
        {
            if (id != product.ProductID) return NotFound();

            if (!ModelState.IsValid)
            {
                try
                {
                   
                    if (formFile != null && formFile.Length > 0)
                    {
                        
                        if (!string.IsNullOrEmpty(product.ProductImage))
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ProductImage.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                       
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Photos/products");
                        string uniqueFileName = product.ProductSKU + Path.GetExtension(formFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(fileStream);
                        }
                        
                        product.ProductImage = uniqueFileName;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductModelId"] = new SelectList(_context.ProductModel, "ProductModelID", "ProductModelName");
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Product
                .Include(p => p.ProductModel)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null) return NotFound();
            return View(product);
        }

       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                
                if (!string.IsNullOrEmpty(product.ProductImage))
                {
          

                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Photos/products",product.ProductImage.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Products/GetProductInfo/5
        [HttpGet]
      
        public async Task<IActionResult> GetProductInfo(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Json(new { isSerialized = product.IsSerialized });
        }

        [HttpGet]
      
        public async Task<IActionResult> SearchProducts(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(new List<ProductSearchDTO>());
            }

            var products = await _context.ProductDetail
                .Include(pd => pd.Product)
                    .ThenInclude(p => p.ProductModel)
                        .ThenInclude(pm => pm.ModelSpec)
                .Where(pd => pd.Status == "庫存中" &&
                    (
                        pd.Product.ProductName.Contains(keyword) ||
                        pd.Product.ProductSKU.Contains(keyword) ||
                        pd.Product.ProductModel.ModelSpec.Any(ms => ms.SpecValue.Contains(keyword))
                    )
                )
                .Select(pd => new ProductSearchDTO
                {
                    ProductDetailID = pd.ProductDetailID,
                    Label = pd.Product.ProductName + " (" +
                            string.Join(", ", pd.Product.ProductModel.ModelSpec.Select(ms => ms.SpecValue)) +
                            ")",
                    Price = pd.Price,
                })
                .Take(10)
                .ToListAsync();

            return Json(products);
        }
       
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }

    }

}