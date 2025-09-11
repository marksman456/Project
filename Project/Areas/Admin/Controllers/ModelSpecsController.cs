using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectData.Data;
using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ModelSpecsController : Controller
    {
        private readonly XiangYunDbContext _context;

        public ModelSpecsController(XiangYunDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ModelSpecs
        public async Task<IActionResult> Index()
        {
            var xiangYunDbContext = _context.ModelSpec.Include(m => m.ProductModel);
            return View(await xiangYunDbContext.ToListAsync());
        }

        // GET: Admin/ModelSpecs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelSpec = await _context.ModelSpec
                .Include(m => m.ProductModel)
                .FirstOrDefaultAsync(m => m.ModelSpecID == id);
            if (modelSpec == null)
            {
                return NotFound();
            }

            return View(modelSpec);
        }

        // GET: Admin/ModelSpecs/Create
        public IActionResult Create()
        {
            ViewData["ProductModelID"] = new SelectList(_context.ProductModel, "ProductModelID", "ProductModelName");
            return View();
        }

        // POST: Admin/ModelSpecs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModelSpecID,ProductModelID,Spec,SpecValue")] ModelSpec modelSpec)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelSpec);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductModelID"] = new SelectList(_context.ProductModel, "ProductModelID", "ProductModelName", modelSpec.ProductModelID);
            return View(modelSpec);
        }

        // GET: Admin/ModelSpecs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelSpec = await _context.ModelSpec.FindAsync(id);
            if (modelSpec == null)
            {
                return NotFound();
            }
            ViewData["ProductModelID"] = new SelectList(_context.ProductModel, "ProductModelID", "ProductModelName", modelSpec.ProductModelID);
            return View(modelSpec);
        }

        // POST: Admin/ModelSpecs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModelSpecID,ProductModelID,Spec,SpecValue")] ModelSpec modelSpec)
        {
            if (id != modelSpec.ModelSpecID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelSpec);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelSpecExists(modelSpec.ModelSpecID))
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
            ViewData["ProductModelID"] = new SelectList(_context.ProductModel, "ProductModelID", "ProductModelName", modelSpec.ProductModelID);
            return View(modelSpec);
        }

      
       

  
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modelSpec = await _context.ModelSpec.FindAsync(id);
            if (modelSpec != null)
            {
                _context.ModelSpec.Remove(modelSpec);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModelSpecExists(int id)
        {
            return _context.ModelSpec.Any(e => e.ModelSpecID == id);
        }
    }
}
