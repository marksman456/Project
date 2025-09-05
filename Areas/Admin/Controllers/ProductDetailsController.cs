using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectData.Models;
using Project.ViewModels;
using ProjectData.Data;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductDetailsController : Controller
    {
        private readonly XiangYunDbContext _context;

        public ProductDetailsController(XiangYunDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductDetails
       
          public async Task<IActionResult> Index()
        {
            var allStock = await _context.ProductDetail
                .Include(p => p.Product)
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            var viewModel = new ProductDetailIndexViewModel
            {
                SerializedItems = allStock.Where(p => p.Product.IsSerialized).ToList(),
                NonSerializedItems = allStock.Where(p => !p.Product.IsSerialized).ToList()
            };

            return View(viewModel);
        }
        

        // GET: Admin/ProductDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetail
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductDetailID == id);
            if (productDetail == null)
            {
                return NotFound();
            }

            return View(productDetail);
        }

      
        public IActionResult Create()
        {
            ViewBag.Products = new SelectList(_context.Product, "ProductID", "ProductName");
            return View();
        }

        // POST: ProductDetails/Create
        // POST: ProductDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,SerialNumber,PurchaseDate,Price,Quantity")] ProductDetail productDetail)
        {
            ModelState.Remove("Status");
            ModelState.Remove("Product");

            var product = await _context.Product.FindAsync(productDetail.ProductID);
            if (product == null)
            {
                ModelState.AddModelError("ProductID", "找不到對應的產品系列。");
            }
            else if (product.IsSerialized)
            {
                if (string.IsNullOrWhiteSpace(productDetail.SerialNumber))
                {
                    ModelState.AddModelError("SerialNumber", "序號化商品必須填寫產品序號。");
                }
                else
                {
                    var isDuplicate = await _context.ProductDetail.AnyAsync(pd => pd.SerialNumber == productDetail.SerialNumber);
                    if (isDuplicate)
                    {
                        ModelState.AddModelError("SerialNumber", "此產品序號已存在。");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    if (product.IsSerialized)
                    {
                        // --- 序號化商品邏輯 ---
                        productDetail.Status = "庫存中";
                        productDetail.Quantity = 1;

                        var movement = new InventoryMovement
                        {
                            MovementDate = DateTime.Now,
                            MovementType = "進貨",
                            Quantity = 1,
                            ProductDetail = productDetail // 直接關聯到上面的 productDetail 物件
                        };

                        _context.Add(productDetail); // 準備新增 ProductDetail
                        _context.Add(movement);      // 準備新增 InventoryMovement
                    }
                    else
                    {
                        // --- 非序號化商品邏輯 ---
                        var existingStock = await _context.ProductDetail.FirstOrDefaultAsync(pd => pd.ProductID == productDetail.ProductID && pd.SerialNumber == null);

                        InventoryMovement movement;

                        if (existingStock != null)
                        {
                            // 如果庫存已存在，則更新數量
                            existingStock.Quantity += productDetail.Quantity;
                            _context.Update(existingStock);

                            movement = new InventoryMovement
                            {
                                MovementDate = DateTime.Now,
                                MovementType = "進貨",
                                Quantity = productDetail.Quantity, // 異動數量是本次新增的數量
                                ProductDetail = existingStock // 關聯到「已存在」的庫存紀錄
                            };
                        }
                        else
                        {
                            // 如果庫存不存在，則建立新的
                            productDetail.Status = "庫存中";
                            productDetail.SerialNumber = null;

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

                    TempData["SuccessMessage"] = "庫存品項新增成功！";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    string errorMessage = ex.Message;
                    if (ex.InnerException != null)
                    {
                        errorMessage += " |---> Inner Exception: " + ex.InnerException.Message;
                    }
                    ModelState.AddModelError(string.Empty, $"發生未預期的錯誤: {errorMessage}");
                }
            }

            ViewBag.Products = new SelectList(_context.Product, "ProductID", "ProductName", productDetail.ProductID);
            return View(productDetail);
        }
        // GET: Admin/ProductDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productDetail = await _context.ProductDetail.FindAsync(id);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductID", productDetail.ProductID);
            return View(productDetail);
        }

        // POST: Admin/ProductDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductDetailID,ProductID,SerialNumber,PurchaseDate,Status,Quantity,Price")] ProductDetail productDetail)
        {
            if (id != productDetail.ProductDetailID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDetailExists(productDetail.ProductDetailID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Product, "ProductID", "ProductID", productDetail.ProductID);
            return View(productDetail);
        }

       

        private bool ProductDetailExists(int id)
        {
            return _context.ProductDetail.Any(e => e.ProductDetailID == id);
        }
    }
}
