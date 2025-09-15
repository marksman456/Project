
using Project.ViewModels.VMProductDetail;
using System.Threading.Tasks;

namespace Project.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<ProductDetailIndexViewModel> GetInventoryForIndexAsync(string? keyword, string? status);
        Task<ProductDetailCreateViewModel> PrepareNewStockInViewModelAsync();
        Task AddStockAsync(ProductDetailCreateViewModel viewModel);
    }
}