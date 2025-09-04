using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Project.Models;

namespace YourProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // �p�G�ϥΪ̤w�g�n�J�A�����N�L�ɦV���x����O
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