using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var quotations = await _context.Quotation.Include(q => q.Employee).Include(q => q.Member).OrderByDescending(q=>q.CreatedDate).ToListAsync();

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

            return View(quotation);
        }

        // GET: Admin/Quotations/Create
        public IActionResult Create()
        {
            ViewData["EmployeeName"] = new SelectList(_context.Employee, "EmployeeID", "Name");
            ViewData["MemberName"] = new SelectList(_context.Member, "MemberID", "Name");
            var newQuotation = new Quotation
            {
                QuoteDate =DateOnly.FromDateTime(DateTime.Today),
                Status = "報價中" // 預設狀態
            };
            return View(newQuotation);
        }

        // POST: Admin/Quotations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Quotations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 【修改】為了能接收到完整的明細資料，我們稍微調整 Bind 的內容，或直接接收 Quotation 物件
        // 這裡我們直接接收 Quotation 物件，讓模型繫結器自動處理主檔與明細
        public async Task<IActionResult> Create(Quotation quotation)
        {
            // 移除 QuotationNumber 的模型驗證，因為它是後端產生的
            ModelState.Remove("QuotationNumber");
            ModelState.Remove("Status");
            // 手動移除對關聯實體的驗證，因為我們只需要ID
            ModelState.Remove("Member");
            ModelState.Remove("Employee");


            if (quotation.QuotationDetail != null)
            {
                for (int i = 0; i < quotation.QuotationDetail.Count; i++)
                {
                    // 移除明細中對父層 Quotation 物件的驗證
                    ModelState.Remove($"QuotationDetail[{i}].Quotation");
                    // 移除明細中對 ProductDetail 物件的驗證
                    ModelState.Remove($"QuotationDetail[{i}].ProductDetail");
                }
            }

            if (ModelState.IsValid)
            {

                // --- 1. 產生報價單號 (如之前討論的) ---
                string datePart = DateTime.Today.ToString("yyyyMMdd");
                int dailyCount = await _context.Quotation.CountAsync(q => q.QuoteDate == DateOnly.FromDateTime(DateTime.Today));
                string sequencePart = (dailyCount + 1).ToString("D3"); // 格式化為三位數流水號
                quotation.QuotationNumber = $"Q-{datePart}-{sequencePart}";

                // --- 2. 設定預設狀態 ---
                quotation.Status = "報價中";
                quotation.CreatedDate = DateTime.Now; // 設定建立時間
             

                // --- 3. 將組合好的 Quotation 物件 (包含主檔與明細) 加入到 DbContext ---
                _context.Add(quotation);

               

                // --- 4. 執行儲存 ---
                // Entity Framework Core 會非常聰明地處理這一切：
                // a. 它會先執行一筆 INSERT INTO Quotations... 指令來新增主檔。
                // b. 取得剛剛新增的 QuotationID。
                // c. 迭代 quotation.QuotationDetails 集合中的每一個品項。
                // d. 執行多筆 INSERT INTO QuotationDetails... 指令，並自動填入正確的 QuotationID 作為外鍵。
                // e. 這一切都在一個「交易 (Transaction)」中完成，確保資料的一致性。
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // --- 如果 ModelState.IsValid 驗證失敗，需要重新準備下拉選單資料並返回 View ---
            ViewData["MemberName"] = new SelectList(_context.Member, "MemberID", "Name", quotation.MemberID);
            ViewData["EmployeeName"] = new SelectList(_context.Employee, "EmployeeID", "Name", quotation.EmployeeID);
            return View(quotation);
        }

        // GET: Admin/Quotations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quotation = await _context.Quotation.FindAsync(id);
            if (quotation == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmployeeID", quotation.EmployeeID);
            ViewData["MemberID"] = new SelectList(_context.Member, "MemberID", "MemberID", quotation.MemberID);
            return View(quotation);
        }

        // POST: Admin/Quotations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("QuotationID,QuotationNumber,MemberID,EmployeeID,CreatedDate,LastUpdate,QuoteDate,ValidityPeriod,Status,IsTransferred,Note")] Quotation quotation)
        {
            if (id != quotation.QuotationID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quotation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuotationExists(quotation.QuotationID))
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
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmployeeID", quotation.EmployeeID);
            ViewData["MemberID"] = new SelectList(_context.Member, "MemberID", "MemberID", quotation.MemberID);
            return View(quotation);
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
