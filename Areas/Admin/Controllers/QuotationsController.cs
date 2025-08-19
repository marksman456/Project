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
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "Name");
            ViewData["MemberID"] = new SelectList(_context.Member, "MemberID", "Name");
            var newQuotation = new Quotation
            {
                QuoteDate =DateOnly.FromDateTime(DateTime.Now),
                Status = "報價中" // 預設狀態
            };
            return View(newQuotation);
        }

        // POST: Admin/Quotations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("QuotationID,QuotationNumber,MemberID,EmployeeID,CreatedDate,LastUpdate,QuoteDate,ValidityPeriod,Status,IsTransferred,Note")] Quotation quotation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quotation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmployeeID", quotation.EmployeeID);
            ViewData["MemberID"] = new SelectList(_context.Member, "MemberID", "MemberID", quotation.MemberID);
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
