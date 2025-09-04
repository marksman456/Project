using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Project.Models;

namespace YourProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // 如果使用者已經登入，直接將他導向到後台儀表板
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}