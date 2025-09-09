using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectData.Models;
using Project.Services.Interfaces;
using Project.ViewModels.VMQuotation;
using ProjectData.Data;
using static Project.ViewModels.VMQuotation.QuotationViewModel;

namespace Project.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly XiangYunDbContext _context;

        public QuotationService(XiangYunDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuotationViewModel>> GetQuotationsForIndexAsync()
        {
            var quotations = await _context.Quotation
                .Include(q => q.Employee)
                .Include(q => q.Member)
                .Include(q => q.QuotationDetail)
                .Where(q => q.Status == "報價中")
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();

           
            return quotations.Select(q => new QuotationViewModel
            {
                QuotationID = q.QuotationID,
                QuotationNumber = q.QuotationNumber,
                MemberID = q.MemberID,
                EmployeeID = q.EmployeeID,
                MemberName = q.Member.Name,
                EmployeeName = q.Employee.Name,
                CreatedDate = q.CreatedDate,
                QuoteDate = q.QuoteDate,
                ValidityPeriod = q.ValidityPeriod,
                Status = q.Status,
                // 只需顯示，不需下拉選單
                QuotationDetail = q.QuotationDetail.Select(d => new QuotationViewModel.QuotationDetailViewModel
                {
                    QuotationDetailID = d.QuotationID,
                    QuotationID = d.QuotationID,
                    ProductDetailID = d.ProductDetailID,
                    Price = d.Price,
                    Quantity = d.Quantity,
                    Discount = d.Discount
                }).ToList(),
            }).ToList();
        }

        public async Task<QuotationViewModel?> GetQuotationViewModelAsync(int id)
        {
            var quotation = await _context.Quotation
                .Include(q => q.Employee)
                .Include(q => q.Member)
                .Include(q => q.QuotationDetail)
                .ThenInclude(qd => qd.ProductDetail)
                .ThenInclude(pd => pd.Product)
                .ThenInclude(p => p.ProductModel)
                .ThenInclude(pm => pm.ModelSpec)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.QuotationID == id);

         

           
            var viewModel = new QuotationViewModel
            {
                QuotationID = quotation.QuotationID,
                QuotationNumber = quotation.QuotationNumber,
                MemberID = quotation.MemberID,
                EmployeeID = quotation.EmployeeID,
                MemberName = quotation.Member.Name,
                EmployeeName = quotation.Employee.Name,
                QuoteDate = quotation.QuoteDate,
                ValidityPeriod = quotation.ValidityPeriod,
                Note = quotation.Note,
                Status = quotation.Status,
                MemberList = await _context.Member
                    .Select(m => new SelectListItem { Value = m.MemberID.ToString(), Text = m.Name })
                    .ToListAsync(),
                EmployeeList = await _context.Employee
                    .Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Name })
                    .ToListAsync(),
                PaymethodList = _context.Paymethod
                    .Select(p => new SelectListItem { Value = p.PaymethodID.ToString(), Text = p.PaymethodName })
                    .ToList(),
                SalesChannelList = _context.SalesChannel
                    .Select(s => new SelectListItem { Value = s.SalesChannelID.ToString(), Text = s.SalesChannelName })
                    .ToList(),
            QuotationDetail = quotation.QuotationDetail.Select(qd => new QuotationDetailViewModel
                {
                    QuotationDetailID = qd.QuotationID,
                    QuotationID = qd.QuotationID,
                    ProductDetailID = qd.ProductDetailID,
                    Price = qd.Price,
                    Quantity = qd.Quantity,
                    Discount = qd.Discount,
                    ProductStatus = qd.ProductDetail?.Status,
                    ProductNameAndSpec = $"{qd.ProductDetail?.Product?.ProductName} ({string.Join(", ", qd.ProductDetail?.Product?.ProductModel?.ModelSpec.Select(ms => ms.SpecValue) ?? Enumerable.Empty<string>())})"
                }).ToList()
            };

            return viewModel;
        }

        public async Task<QuotationViewModel> PrepareNewQuotationViewModelAsync()
        {
            var viewModel = new QuotationViewModel
            {
                QuoteDate = DateOnly.FromDateTime(DateTime.Today),
                Status = "報價中", // 預設狀態

                MemberList = await _context.Member
                    .Select(m => new SelectListItem { Value = m.MemberID.ToString(), Text = m.Name })
                    .ToListAsync(),
                EmployeeList =await  _context.Employee
                    .Select(e => new SelectListItem { Value = e.EmployeeID.ToString(), Text = e.Name })
                    .ToListAsync()
            };
            return viewModel;
        }

        public async Task<Quotation> CreateQuotationAsync(QuotationViewModel viewModel)
        {
          
            string datePart = DateTime.Today.ToString("yyyyMMdd");
            int dailyCount = await _context.Quotation.CountAsync(q => q.QuoteDate == DateOnly.FromDateTime(DateTime.Today));
            string sequencePart = (dailyCount + 1).ToString("D3"); // 格式化為三位數流水號
            string quotationNumber = $"Q-{datePart}-{sequencePart}";

            var quotation = new Quotation
            {
                QuotationNumber = quotationNumber,
                Status = "報價中",
                CreatedDate = DateTime.Now,
                MemberID = viewModel.MemberID,
                EmployeeID = viewModel.EmployeeID,
                QuoteDate = viewModel.QuoteDate,
                ValidityPeriod = viewModel.ValidityPeriod,
                Note = viewModel.Note,
                QuotationDetail = new List<QuotationDetail>()
            };

            if (viewModel.QuotationDetail != null)
            {
                foreach (var item in viewModel.QuotationDetail)
                {
                    quotation.QuotationDetail.Add(new QuotationDetail
                    {
                        ProductDetailID = item.ProductDetailID,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Discount = item.Discount ?? 0
                    });
                }
            }
            _context.Add(quotation);
            await _context.SaveChangesAsync();
            return quotation;
        }

        public async Task<bool> UpdateQuotationAsync(QuotationViewModel viewModel)
        {
            var quotationInDb = await _context.Quotation
               .Include(q => q.QuotationDetail)
               .FirstOrDefaultAsync(q => q.QuotationID == viewModel.QuotationID);


            quotationInDb.MemberID = viewModel.MemberID;
            quotationInDb.EmployeeID = viewModel.EmployeeID;
            quotationInDb.QuoteDate = viewModel.QuoteDate;
            quotationInDb.ValidityPeriod = viewModel.ValidityPeriod;
            quotationInDb.Status = viewModel.Status;
            quotationInDb.Note = viewModel.Note;
            quotationInDb.LastUpdate = DateTime.Now;




            var detailsToDelete = quotationInDb.QuotationDetail
                    .Where(d => !viewModel.QuotationDetail.Any(vm => vm.QuotationDetailID == d.QuotationID))
                    .ToList();
            _context.QuotationDetail.RemoveRange(detailsToDelete);


            foreach (var detailViewModel in viewModel.QuotationDetail)
            {
                if (detailViewModel.QuotationDetailID > 0) // 更新
                {
                    var detailInDb = quotationInDb.QuotationDetail.FirstOrDefault(d => d.QuotationID == detailViewModel.QuotationDetailID);
                    if (detailInDb != null)
                    {
                        detailInDb.Price = detailViewModel.Price;
                        detailInDb.Quantity = detailViewModel.Quantity;
                        detailInDb.Discount = detailViewModel.Discount;
                    }
                }
                else // 新增
                {
                    quotationInDb.QuotationDetail.Add(new QuotationDetail
                    {
                        ProductDetailID = detailViewModel.ProductDetailID,
                        Price = detailViewModel.Price,
                        Quantity = detailViewModel.Quantity,
                        Discount = detailViewModel.Discount
                    });
                }
            }

  

            await _context.SaveChangesAsync();
            return true ;
        }

        public async Task<bool> VoidQuotationAsync(int id)
        {
            var quotation = await _context.Quotation.FindAsync(id);
            if (quotation == null) return false;
            if (quotation.Status != "報價中") return false; // 只允許「報價中」狀態作廢

            quotation.Status = "已失效";
            quotation.LastUpdate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}