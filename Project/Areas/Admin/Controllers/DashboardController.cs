using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Services.Interfaces;


namespace Project.Areas.Admin.Controllers 
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async  Task<IActionResult> Index()
        {
            var viewModel = await _dashboardService.GetDashboardViewModelAsync();

            return View(viewModel);

        }

    }
}