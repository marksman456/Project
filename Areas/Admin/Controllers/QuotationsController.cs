using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.ViewModels;
using Project.ViewModels.Quotation;
using ProjectData.Data;
using ProjectData.Models;
using static Project.ViewModels.Quotation.QuotationViewModel;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class QuotationsController : Controller
    {
        private readonly XiangYunDbContext _context;

        public QuotationsController(XiangYunDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Quotations
        public async Task<IActionResult> Index()
        {
            var quotations = await _context.Quotation.Include(q => q.Employee).Include(q => q.Member).Include(q=>q.QuotationDetail).OrderByDescending(q=>q.CreatedDate).ToListAsync();

            return View(quotations);
        }

        // GET: Admin/Quotations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotation
                .Include(q => q.Employee)
                .Include(q => q.Member)
                .Include(q=>q.QuotationDetail)
                .ThenInclude(qd=>qd.ProductDetail)
                .ThenInclude(pd=>pd.Product)
                .FirstOrDefaultAsync(m => m.QuotationID == id);
            if (quotation == null)
            {
                return NotFound();
            }

            ViewBag.Paymethods = new SelectList(_context.Paymethod, "PaymethodID", "PaymethodName");
            ViewBag.SalesChannels = new SelectList(_context.SalesChannel, "SalesChannelID", "SalesChannelName");
            return View(quotation);
        }

        // GET: Admin/Quotations/Create
        public IActionResult Create()
        {
            ViewData["EmployeeName"] = new SelectList(_context.Employee, "EmployeeID", "Name");
            ViewData["MemberName"] = new SelectList(_context.Member, "MemberID", "Name");
            var viewModel = new Quotation
            {
                QuoteDate =DateOnly.FromDateTime(DateTime.Today),
                Status = "報價中" // 預設狀態
            };
            return View(viewModel);
        }

        // POST: Admin/Quotations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Quotations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuotationViewModel viewModel)
        {
       

            if (ModelState.IsValid)
            {

                // --- 1. 產生報價單號 (如之前討論的) ---
                string datePart = DateTime.Today.ToString("yyyyMMdd");
                int dailyCount = await _context.Quotation.CountAsync(q => q.QuoteDate == DateOnly.FromDateTime(DateTime.Today));
                string sequencePart = (dailyCount + 1).ToString("D3"); // 格式化為三位數流水號
                string quotationNumber = $"Q-{datePart}-{sequencePart}";

                // --- 2. 設定預設狀態 ---
                viewModel.Status = "報價中";
                viewModel.CreatedDate = DateTime.Now; // 設定建立時間

                var quotation = new Quotation
                {
                    QuotationNumber = quotationNumber,
                    Status = "報價中",
                    CreatedDate = DateTime.Now,
                    MemberID = viewModel.MemberID,
                    EmployeeID = viewModel.EmployeeID,
                    QuoteDate = viewModel.QuoteDate,
                    ValidityPeriod = viewModel.ValidityPeriod,
                    Note = viewModel.Note,
                    QuotationDetail = new List<QuotationDetail>()
                };


                if (viewModel.QuotationDetail != null)
                {
                    foreach (var item in viewModel.QuotationDetail)
                    {
                        quotation.QuotationDetail.Add(new QuotationDetail
                        {
                            ProductDetailID = item.ProductDetailID,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            Discount = item.Discount ?? 0
                        });
                    }
                }
                _context.Add(quotation);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // --- 如果 ModelState.IsValid 驗證失敗，需要重新準備下拉選單資料並返回 View ---
            viewModel.MemberList = _context.Member
             .Select(m => new SelectListItem { Value = m.MemberID.ToString(), Text = m.Name })
             .ToList();

            viewModel.EmployeeList = _context.Employee
                .Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Name })
                .ToList();
            return View(viewModel);
        }

        // GET: Admin/Quotations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var quotation = await _context.Quotation
                .Include(q => q.QuotationDetail)
                    .ThenInclude(qd => qd.ProductDetail)
                        .ThenInclude(pd => pd.Product)
                            .ThenInclude(p => p.ProductModel)
                                .ThenInclude(pm => pm.ModelSpec)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.QuotationID == id);

            if (quotation == null) return NotFound();

            // 【關鍵點 #1】: 將從資料庫撈出的 Quotation，手動映射(Mapping)成 ViewModel
            var viewModel = new QuotationViewModel
            {
                QuotationID = quotation.QuotationID,
                QuotationNumber = quotation.QuotationNumber,
                MemberID = quotation.MemberID,
                EmployeeID = quotation.EmployeeID,
                CreatedDate = quotation.CreatedDate,
                LastUpdate = quotation.LastUpdate,
                QuoteDate = quotation.QuoteDate,
                ValidityPeriod = quotation.ValidityPeriod,
                Status = quotation.Status,
                Note = quotation.Note,
                QuotationDetail = quotation.QuotationDetail.Select(qd => new QuotationDetailViewModel
                {
                    QuotationDetailID = qd.QuotationID,
                    QuotationID = qd.QuotationID,
                    ProductDetailID = qd.ProductDetailID,
                    Price = qd.Price,
                    Quantity = qd.Quantity,
                    Discount = qd.Discount,
                    ProductNameAndSpec = $"{qd.ProductDetail?.Product?.ProductName} ({string.Join(", ", qd.ProductDetail?.Product?.ProductModel?.ModelSpec.Select(ms => ms.SpecValue) ?? Enumerable.Empty<string>())})"
                }).ToList()
            };

            ViewData["MemberName"] = new SelectList(_context.Member, "MemberID", "Name", viewModel.MemberID);
            ViewData["EmployeeName"] = new SelectList(_context.Employee, "EmployeeID", "Name", viewModel.EmployeeID);

            // 傳給 View 的是 viewModel，不是 quotation
            return View(viewModel);
        }

        // POST: Admin/Quotations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Admin/Quotations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuotationViewModel viewModel)
        {


            //debug用
            System.Diagnostics.Debug.WriteLine("=== POST Edit 方法已進入 ===");


            if (id != viewModel.QuotationID) return NotFound();

            if (ModelState.IsValid)
            {
                var quotationInDb = await _context.Quotation
                    .Include(q => q.QuotationDetail)
                    .FirstOrDefaultAsync(q => q.QuotationID == id);

                if (quotationInDb == null)
                {
                    return NotFound();
                }


                // --- 將 ViewModel 的變動，手動映射回 Entity Model ---
                quotationInDb.MemberID = viewModel.MemberID;
                quotationInDb.EmployeeID = viewModel.EmployeeID;
                quotationInDb.QuoteDate = viewModel.QuoteDate;
                quotationInDb.ValidityPeriod = viewModel.ValidityPeriod;
                quotationInDb.Status = viewModel.Status;
                quotationInDb.Note = viewModel.Note;
                quotationInDb.LastUpdate = DateTime.Now;

                var detailsToDelete = quotationInDb.QuotationDetail
                    .Where(d => !viewModel.QuotationDetail.Any(vm => vm.QuotationDetailID == d.QuotationID))
                    .ToList();
                _context.QuotationDetail.RemoveRange(detailsToDelete);

                foreach (var detailViewModel in viewModel.QuotationDetail)
                {
                    if (detailViewModel.QuotationDetailID > 0) // 更新
                    {
                        var detailInDb = quotationInDb.QuotationDetail.FirstOrDefault(d => d.QuotationID == detailViewModel.QuotationDetailID);
                        if (detailInDb != null)
                        {
                            detailInDb.Price = detailViewModel.Price;
                            detailInDb.Quantity = detailViewModel.Quantity;
                            detailInDb.Discount = detailViewModel.Discount;
                        }
                    }
                    else // 新增
                    {
                        quotationInDb.QuotationDetail.Add(new QuotationDetail
                        {
                            ProductDetailID = detailViewModel.ProductDetailID,
                            Price = detailViewModel.Price,
                            Quantity = detailViewModel.Quantity,
                            Discount = detailViewModel.Discount
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 【關鍵點 #2】: 如果驗證失敗，也要重新準備下拉選單，並將 viewModel 傳回
            ViewData["MemberName"] = new SelectList(_context.Member, "MemberID", "Name", viewModel.MemberID);
            ViewData["EmployeeName"] = new SelectList(_context.Employee, "EmployeeID", "Name", viewModel.EmployeeID);

            // 傳給 View 的是 viewModel
            return View(viewModel);
        }




        // GET: Admin/Quotations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotation
                .Include(q => q.Employee)
                .Include(q => q.Member)
                .FirstOrDefaultAsync(m => m.QuotationID == id);
            if (quotation == null)
            {
                return NotFound();
            }

            return View(quotation);
        }

        // POST: Admin/Quotations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quotation = await _context.Quotation.FindAsync(id);
            if (quotation != null)
            {
                _context.Quotation.Remove(quotation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuotationExists(int id)
        {
            return _context.Quotation.Any(e => e.QuotationID == id);
        }
    }
}
