using Project.ViewModels.VMQuotation;
using ProjectData.Models;

namespace Project.Services.Interfaces
{
    public interface IQuotationService
    {
        Task<List<QuotationViewModel>> GetQuotationsForIndexAsync();
        Task<QuotationViewModel?> GetQuotationViewModelAsync(int id);
        Task<QuotationViewModel> PrepareNewQuotationViewModelAsync();
        Task<Quotation> CreateQuotationAsync(QuotationViewModel viewModel);
        Task<bool> UpdateQuotationAsync(QuotationViewModel viewModel);

        Task<bool> VoidQuotationAsync(int id);
    }
}
