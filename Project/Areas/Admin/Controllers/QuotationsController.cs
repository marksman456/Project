using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Services.Interfaces;
using Project.ViewModels;
using Project.ViewModels.VMQuotation;
using ProjectData.Data;
using ProjectData.Models;
using static Project.ViewModels.VMQuotation.QuotationViewModel;

namespace Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Salesperson")]
    public class QuotationsController : Controller
    {
        private readonly IQuotationService _quotationService;

        public QuotationsController(IQuotationService quotationService)
        {
            _quotationService = quotationService;
        }

        // GET: Admin/Quotations
        public async Task<IActionResult> Index()
        {
            var viewModelList = await _quotationService.GetQuotationsForIndexAsync();

            return View(viewModelList);
        }

        // GET: Admin/Quotations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _quotationService.GetQuotationViewModelAsync(id.Value);

            if (viewModel == null)
            {
                return NotFound();
            }


            return View(viewModel);
        }

        // GET: Admin/Quotations/Create
        public async Task<IActionResult> Create()
        {

            var viewModel =await _quotationService.PrepareNewQuotationViewModelAsync();


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuotationViewModel viewModel)
        {
            if (viewModel == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var createdQuotation = await _quotationService.CreateQuotationAsync(viewModel);
                TempData["SuccessMessage"] = "報價單已成功建立。";
                return RedirectToAction(nameof(Index));
            }
            var repopulatedViewModel = await _quotationService.PrepareNewQuotationViewModelAsync();


            return View(viewModel);
        }

        // GET: Admin/Quotations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _quotationService.GetQuotationViewModelAsync(id.Value);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuotationViewModel viewModel)
        {

            if (id != viewModel.QuotationID)
            {
                return NotFound();

            }

            if (ModelState.IsValid)
            {

                var success = await _quotationService.UpdateQuotationAsync(viewModel);


                if (!success)
                {
                    return NotFound();
                }



                TempData["SuccessMessage"] = "報價單已成功更新。";



                return RedirectToAction(nameof(Index));
            }
            var repopulatedViewModel = await _quotationService.PrepareNewQuotationViewModelAsync();

            return View(viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Void(int id)
        {
            var success = await _quotationService.VoidQuotationAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "報價單已成功作廢。";
            }
            else
            {
                TempData["ErrorMessage"] = "操作失敗，報價單可能已成交或狀態不符。";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
