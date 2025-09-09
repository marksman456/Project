using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Services.Interfaces;
using Project.ViewModels.VMEmployee;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {

        private readonly IEmployeeService _employeeService; 


        public EmployeesController(IEmployeeService employeeService) 
        {
            _employeeService = employeeService;
        }

        // GET: Admin/Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetEmployeesForIndexAsync();
            return View(employees);
        }

        // GET: Admin/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _employeeService.GetEmployeeDetailsViewModelAsync(id.Value);
               
            if (viewModel == null)
            {
                return NotFound();
            }

      

            return View(viewModel);
        }

        // GET: Admin/Employees/Create
        // GET: Admin/Employees/Create
        public async Task<IActionResult> Create()
        {
         var genderList = new List<SelectListItem>
         {
             new() { Value = "M", Text = "男性" },
             new() { Value = "F", Text = "女性" },
             new() { Value = "O", Text = "其他" }
         };

         ViewBag.GenderList = genderList;
            

            ViewData["UserId"] = await _employeeService.GetAvailableUsersForDropdownAsync();


            return View(new EmployeeViewModel());
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.CreateEmployeeAsync(viewModel);
                TempData["SuccessMessage"] = "員工資料已成功建立。";
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = await _employeeService.GetAvailableUsersForDropdownAsync();

            return View(viewModel);
        }

       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _employeeService.GetEmployeeEditViewModelAsync(id.Value);

            if (viewModel == null)
            {
                return NotFound();
            }

         var genderList = new List<SelectListItem>
         {
            new() { Value = "M", Text = "男性" },
            new() { Value = "F", Text = "女性" },
            new() { Value = "O", Text = "其他" }
         };

        ViewBag.GenderList = genderList;

            // 用 employee.UserId 來設定下拉選單的預設選中項
            ViewData["UserId"] = await _employeeService.GetAvailableUsersForDropdownAsync(id.Value);
            return View(viewModel);
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeViewModel viewModel)
        {
            if (id != viewModel.EmployeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                  var updateResult = await _employeeService.UpdateEmployeeAsync(viewModel);

                    if (!updateResult)
                    {
                        return NotFound();
                    }

                    TempData["SuccessMessage"] = "員工資料已成功更新。";
                    return RedirectToAction(nameof(Index));
                
            }
            ViewData["UserId"] = await _employeeService.GetAvailableUsersForDropdownAsync(id);
            return View(viewModel);
        }
    }
}
