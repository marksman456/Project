using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.ViewModels;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]//  [Authorize(Roles = "Admin")] 來限制只有管理員能訪問
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }


        // GET: Admin/Users/ManageRoles/some-guid
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            foreach (var role in _roleManager.Roles.ToList())
            {
                viewModel.Roles.Add(new RoleSelection
                {
                    RoleName = role.Name,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
                });
            }

            return View(viewModel);
        }

        // POST: Admin/Users/ManageRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel viewModel)
        {
            var user = await _userManager.FindByIdAsync(viewModel.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = viewModel.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

            // 找出需要新增的角色 (新選的 - 目前有的)
            var rolesToAdd = selectedRoles.Except(currentRoles);
            var result = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "為使用者新增角色時發生錯誤。");
                return View(viewModel);
            }

            // 找出需要移除的角色 (目前有的 - 新選的)
            var rolesToRemove = currentRoles.Except(selectedRoles);
            result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "移除使用者現有角色時發生錯誤。");
                return View(viewModel);
            }

            TempData["SuccessMessage"] = "使用者角色已成功更新。";
            return RedirectToAction(nameof(Index));
        }
    }
}