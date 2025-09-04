using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly XiangYunDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // 【新增】

        public EmployeesController(XiangYunDbContext context, UserManager<IdentityUser> userManager) // 【修改】
        {
            _context = context;
            _userManager = userManager; // 【新增】
        }

        // GET: Admin/Employees
        public async Task<IActionResult> Index()
        {
            var xiangYunDbContext = _context.Employee.Include(e => e.User);
            return View(await xiangYunDbContext.ToListAsync());
        }

        // GET: Admin/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Admin/Employees/Create
        // GET: Admin/Employees/Create
        public async Task<IActionResult> Create()
        {
            // 找出所有已被指派的 UserId
            var assignedUserIds = await _context.Employee
                                        .Where(e => e.UserId != null)
                                        .Select(e => e.UserId)
                                        .ToListAsync();

            // 找出所有系統使用者，並排除已被指派的
            var availableUsers = await _userManager.Users
                                        .Where(u => !assignedUserIds.Contains(u.Id))
                                        .ToListAsync();

            var genderList = new List<SelectListItem>
          {
        new() { Value = "M", Text = "男性" },
        new() { Value = "F", Text = "女性" },
        new() { Value = "O", Text = "其他" }
             };

            ViewBag.GenderList = genderList;

            ViewData["UserId"] = new SelectList(availableUsers, "Id", "UserName");
            return View();
        }

        // POST: Admin/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeID,EmployeeNumber,Name,Phone,Address,Email,Gender,Birthday,HireDate,Resignation,Note,UserId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Id", employee.UserId);
            return View(employee);
        }

        // GET: Admin/Employees/Edit/5
        // GET: Admin/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            // 找出除了目前這位員工之外，其他已被指派的 UserId
            var otherAssignedUserIds = await _context.Employee
                                            .Where(e => e.EmployeeID != id && e.UserId != null)
                                            .Select(e => e.UserId)
                                            .ToListAsync();

            // 找出所有系統使用者，並排除上面那些已被其他人指派的
            var availableUsers = await _userManager.Users
                                            .Where(u => !otherAssignedUserIds.Contains(u.Id))
                                            .ToListAsync();

                       var genderList = new List<SelectListItem>
          {
        new() { Value = "M", Text = "男性" },
        new() { Value = "F", Text = "女性" },
        new() { Value = "O", Text = "其他" }
             };

            ViewBag.GenderList = genderList;

            // 用 employee.UserId 來設定下拉選單的預設選中項
            ViewData["UserId"] = new SelectList(availableUsers, "Id", "UserName", employee.UserId);
            return View(employee);
        }
        // POST: Admin/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,EmployeeNumber,Name,Phone,Address,Email,Gender,Birthday,HireDate,Resignation,Note,UserId, Title")] Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeID))
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
            ViewData["UserId"] = new SelectList(_context.Set<IdentityUser>(), "Id", "Id", employee.UserId);
            return View(employee);
        }

       

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.EmployeeID == id);
        }
    }
}
