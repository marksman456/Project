using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Models.DTOs;
using Project.Services.Interfaces;
using Project.ViewModels.VMProduct;
using ProjectData.Data;
using ProjectData.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Services
{
    // 備註：這個 Service 負責所有與產品相關的商業邏輯，包含檔案處理。
    public class ProductService : IProductService
    {
        private readonly XiangYunDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(XiangYunDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // --- Read Operations ---
        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Product.Include(p => p.ProductModel).ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Product
                .Include(p => p.ProductModel)
                .FirstOrDefaultAsync(m => m.ProductID == id);
        }

        // --- Create Operations ---
        public async Task<ProductCreateViewModel> PrepareNewProductViewModelAsync()
        {
            return new ProductCreateViewModel
            {
                ProductModelList = new SelectList(await _context.ProductModel.ToListAsync(), "ProductModelID", "ProductModelName")
            };
        }

        public async Task CreateProductAsync(ProductCreateViewModel viewModel)
        {
            var product = new Product
            {
                ProductSKU = viewModel.ProductSKU,
                ProductName = viewModel.ProductName,
                Description = viewModel.Description,
                ProductModelID = viewModel.ProductModelID,
                Price = viewModel.Price,
                IsSerialized = viewModel.IsSerialized,
                // 預設為空，稍後處理圖片上傳
            };

            if (viewModel.ProductImageFile != null)
            {
                product.ProductImage = await SaveImageFile(viewModel.ProductImageFile, product.ProductSKU);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();
        }

        // --- Update Operations ---
        public async Task<ProductEditViewModel?> PrepareEditProductViewModelAsync(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null) return null;

            return new ProductEditViewModel
            {
                ProductID = product.ProductID,
                ProductSKU = product.ProductSKU,
                ProductName = product.ProductName,
                Description = product.Description,
                ProductModelID = product.ProductModelID,
                Price = product.Price,
                IsSerialized = product.IsSerialized,
                ExistingImagePath = product.ProductImage,
                ProductModelList = new SelectList(await _context.ProductModel.ToListAsync(), "ProductModelID", "ProductModelName", product.ProductModelID)
            };
        }

        public async Task UpdateProductAsync(ProductEditViewModel viewModel)
        {
            var product = await _context.Product.FindAsync(viewModel.ProductID);
            if (product == null) throw new System.Exception("找不到該產品");

            product.ProductSKU = viewModel.ProductSKU;
            product.ProductName = viewModel.ProductName;
            product.Description = viewModel.Description;
            product.ProductModelID = viewModel.ProductModelID;
            product.Price = viewModel.Price;
            product.IsSerialized = viewModel.IsSerialized;
            // 處理圖片更新
            product.ProductImage = viewModel.ExistingImagePath; // 預設為現有圖片


            if (viewModel.ProductImageFile != null)
            {
                if (!string.IsNullOrEmpty(product.ProductImage))
                {
                    DeleteImageFile(product.ProductImage);
                }
                product.ProductImage = await SaveImageFile(viewModel.ProductImageFile, product.ProductSKU);
            }

            _context.Update(product);
            await _context.SaveChangesAsync();
        }

        // --- Delete Operation ---
        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ProductImage))
                {
                    DeleteImageFile(product.ProductImage);
                }
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // --- Search API ---
        public async Task<List<ProductSearchDTO>> SearchAvailableProductsAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return new List<ProductSearchDTO>();
            }

            var products = await _context.ProductDetail
                .Include(pd => pd.Product)
                    .ThenInclude(p => p.ProductModel)
                        .ThenInclude(pm => pm.ModelSpec)
                .Where(pd => pd.Status == "庫存中" &&
                    (
                        pd.Product.ProductName.Contains(keyword) ||
                        pd.Product.ProductSKU.Contains(keyword) ||
                            (pd.Product.ProductModel != null && pd.Product.ProductModel.ModelSpec.Any(ms => ms.SpecValue.Contains(keyword)))
                    )
                )
                .Select(pd => new ProductSearchDTO
                {
                    ProductDetailID = pd.ProductDetailID,
                    Label = (pd.Product.ProductModel != null && pd.Product.ProductModel.ModelSpec.Any())
                    ? pd.Product.ProductName + " (" + string.Join(", ", pd.Product.ProductModel.ModelSpec.Select(ms => ms.SpecValue)) + ")" + (pd.SerialNumber != null && pd.SerialNumber.StartsWith("BATCH-") ? " - 批號:" + pd.SerialNumber.Substring(20) : "")
                    : pd.Product.ProductName + (pd.SerialNumber != null && pd.SerialNumber.StartsWith("BATCH-") ? " - 批號:" + pd.SerialNumber.Substring(20) : ""),
                    Price = pd.Price,
                })
                .Take(10)
                .ToListAsync();

            return products;
        }

        // --- Private Helper Methods for File I/O ---
        private async Task<string> SaveImageFile(IFormFile formFile, string productSku)
        {
            if (formFile == null) throw new ArgumentNullException(nameof(formFile));

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Photos", "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 只允許常見圖片副檔名
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var ext = Path.GetExtension(formFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("不支援的圖片格式");

            // 避免檔名衝突
            string uniqueFileName = productSku + Path.GetExtension(formFile.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }
            return uniqueFileName;
        }

        private void DeleteImageFile(string fileName)
        {
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Photos", "products", fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}