
using Project.ViewModels.VMProductDetail;
using System.Threading.Tasks;

namespace Project.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<ProductDetailIndexViewModel> GetInventoryForIndexAsync();
        Task<ProductDetailCreateViewModel> PrepareNewStockInViewModelAsync();
        Task AddStockAsync(ProductDetailCreateViewModel viewModel);
    }
}