using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Services.Interfaces;
using Project.ViewModels.VMProductDetail;
using ProjectData.Data;
using ProjectData.Models;

namespace Project.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly XiangYunDbContext _context;

        public InventoryService(XiangYunDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDetailIndexViewModel> GetInventoryForIndexAsync()
        {
            var allStock = await _context.ProductDetail
                .Include(p => p.Product)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return new ProductDetailIndexViewModel
            {
                SerializedItems = allStock.Where(p => p.Product.IsSerialized).ToList(),
                NonSerializedItems = allStock.Where(p => !p.Product.IsSerialized).ToList()
            };
        }

        public async Task<ProductDetailCreateViewModel> PrepareNewStockInViewModelAsync()
        {
            return new ProductDetailCreateViewModel
            {
                ProductList = new SelectList(await _context.Product.ToListAsync(), "ProductID", "ProductName")
            };
        }

        public async Task AddStockAsync(ProductDetailCreateViewModel viewModel)
        {
            var product = await _context.Product.FindAsync(viewModel.ProductID);
            if (product == null)
            {
                throw new InvalidOperationException("找不到對應的產品系列。");
            }

            if (product.IsSerialized)
            {
                if (string.IsNullOrWhiteSpace(viewModel.SerialNumber))
                {
                    throw new InvalidOperationException("序號化商品必須填寫產品序號。");
                }
                var isDuplicate = await _context.ProductDetail.AnyAsync(pd => pd.SerialNumber == viewModel.SerialNumber);
                if (isDuplicate)
                {
                    throw new InvalidOperationException("此產品序號已存在。");
                }
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                product.Price = viewModel.SalePrice; // 更新主檔最新成本
                _context.Update(product);

                if (product.IsSerialized)
                {
                    var productDetail = new ProductDetail
                    {
                        ProductID = viewModel.ProductID,
                        SerialNumber = viewModel.SerialNumber,
                        PurchaseDate = viewModel.PurchaseDate,
                        PurchaseCost = viewModel.PurchaseCost,
                        Price = viewModel.SalePrice, //儲存獨立售價
                        Status = "庫存中",
                        Quantity = 1,
                    };



                    var movement = new InventoryMovement
                    {
                        MovementDate = DateTime.Now,
                        MovementType = "進貨",
                        Quantity = 1,
                        ProductDetail = productDetail
                    };

                    _context.Add(movement);
                    _context.Add(productDetail);
                }
                else
                {
                    var existingStock = await _context.ProductDetail.FirstOrDefaultAsync(pd => pd.ProductID == viewModel.ProductID && pd.SerialNumber == null);

                    InventoryMovement movement;
                    if (existingStock != null)
                    {
                        // 如果庫存已存在，則更新數量
                        existingStock.Quantity += viewModel.Quantity;
                        _context.Update(existingStock);

                        movement = new InventoryMovement
                        {
                            MovementDate = DateTime.Now,
                            MovementType = "進貨",
                            Quantity = viewModel.Quantity, // 異動數量是本次新增的數量
                            ProductDetail = existingStock // 關聯到「已存在」的庫存紀錄
                        };
                    }
                    else
                    {
                        var productDetail = new ProductDetail
                        {
                            ProductID = viewModel.ProductID,
                            SerialNumber = null,
                            PurchaseDate = viewModel.PurchaseDate,
                            PurchaseCost = viewModel.PurchaseCost,
                            Price = viewModel.SalePrice, //儲存獨立售價
                            Status = "庫存中",
                            Quantity = viewModel.Quantity
                        };

                        movement = new InventoryMovement
                        {
                            MovementDate = DateTime.Now,
                            MovementType = "進貨",
                            Quantity = productDetail.Quantity,
                            ProductDetail = productDetail // 關聯到「即將新增」的庫存紀錄
                        };

                        _context.Add(productDetail); // 準備新增 ProductDetail

                    }
                    _context.Add(movement); // 準備新增 InventoryMovement
                }

                // 在這裡，一次性將所有準備好的變更寫入資料庫
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }

            catch (Exception)
            {
                // 如果發生任何錯誤，復原所有變更
                await transaction.RollbackAsync();
                // 將錯誤重新拋出，讓 Controller 捕捉
                throw;
            }

        }

    }
}
    
