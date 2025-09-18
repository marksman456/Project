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

        public async Task<ProductDetailIndexViewModel> GetInventoryForIndexAsync(string? keyword, string? status)
        {
            var query = _context.ProductDetail
                                   .Include(p => p.Product)
                                   .AsQueryable(); // 使用 AsQueryable() 來延遲執行，以便後續附加條件

            // --- 動態加入篩選條件 ---

            // 1. 根據「狀態」進行篩選
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(pd => pd.Status == status);
            }

            // 2. 根據「關鍵字」進行搜尋
            if (!string.IsNullOrEmpty(keyword))
            {
                // 這裡的 keyword 會同時搜尋產品名稱、SKU、以及最重要的 SerialNumber/批號
                query = query.Where(pd =>
                    pd.Product.ProductName.Contains(keyword) ||
                    pd.SerialNumber.Contains(keyword) ||
                    pd.Product.ProductSKU.Contains(keyword));
            }

            // 在所有條件都加上後，才執行查詢並排序
            var filteredStock = await query
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            // 後續的邏輯不變，將篩選後的結果，再分成兩種商品類型
            return new ProductDetailIndexViewModel
            {
                SerializedItems = filteredStock.Where(p => p.Product.IsSerialized).ToList(),
                NonSerializedItems = filteredStock.Where(p => !p.Product.IsSerialized).ToList()
            };
        }

        public async Task<ProductDetailCreateViewModel> PrepareNewStockInViewModelAsync()
        {
            var products = await _context.Product
                           //透過 Include，一併將每個產品關聯的 Specs 讀取出來
                           .Include(p => p.Specs)
                           .OrderBy(p => p.ProductName)
                           .ToListAsync();

            return new ProductDetailCreateViewModel
            {
                ProductList = new SelectList(products, "ProductID", "ProductNameWithSpec"),
                PurchaseDate = DateOnly.FromDateTime(DateTime.Now)
            };
        }

        public async Task AddStockAsync(ProductDetailCreateViewModel viewModel)
        {
            var product = await _context.Product.FindAsync(viewModel.ProductID);
            if (product == null)
            {
                throw new InvalidOperationException("找不到對應的產品系列。");
            }

            string identifier; // 這個變數將儲存「真實序號」或「虛擬批號」
            int quantityToSave;

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

                identifier = viewModel.SerialNumber;
                quantityToSave = 1;
            }
            else
            {
                // 對於批量商品，由系統產生一個唯一的「批號」
                // 格式：BATCH-日期-GUID前8碼 (例如 BATCH-20250911-A8E5C1D3)
                identifier = $"BATCH-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
                quantityToSave = viewModel.Quantity;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                if (product.IsSerialized)
                {
                   

                    _context.Update(product);
                    var productDetail = new ProductDetail
                    {
                        ProductID = viewModel.ProductID,
                        SerialNumber = viewModel.SerialNumber,
                        PurchaseDate = viewModel.PurchaseDate,
                        PurchaseCost = viewModel.PurchaseCost,
                        Price = viewModel.SalePrice, //儲存獨立售價
                        Status = "庫存中",
                        Quantity = quantityToSave,
                    };



                    var movement = new InventoryMovement
                    {
                        MovementDate = DateTime.Now,
                        MovementType = "進貨",
                        Quantity = quantityToSave,
                        ProductDetail = productDetail
                    };

                    _context.Add(movement);
                    _context.Add(productDetail);
                }
              
                    
                    else
                    {
                        var productDetail = new ProductDetail
                        {
                            ProductID = viewModel.ProductID,
                            SerialNumber = identifier,
                            PurchaseDate = viewModel.PurchaseDate,
                            PurchaseCost = viewModel.PurchaseCost,
                            Price = product.Price,
                            Status = "庫存中",
                            Quantity = quantityToSave
                        };

                      var  movement = new InventoryMovement
                        {
                            MovementDate = DateTime.Now,
                            MovementType = "進貨",
                            Quantity = viewModel.Quantity,
                            ProductDetail = productDetail // 關聯到「即將新增」的庫存紀錄
                        };

                        _context.Add(productDetail); // 準備新增 ProductDetail


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
    
