using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Project.Areas.Admin.Controllers 
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] 
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        // 透過建構函式，注入 RoleManager
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }


        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = $"角色 {roleName} 已成功建立。";
                        return RedirectToAction(nameof(Index));
                    }
                    // 如果建立失敗，將 Identity 的錯誤訊息加到 ModelState
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "此角色名稱已存在。");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "角色名稱不可為空。");
            }

            // 如果有任何錯誤，則回到 Create 頁面並顯示錯誤
            return View((object)roleName);
        }
    }
}