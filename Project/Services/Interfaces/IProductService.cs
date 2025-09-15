using Project.Models.DTOs;
using Project.ViewModels.VMProduct;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProjectData.Models.Product>> GetAllProductsAsync();
        Task<ProjectData.Models.Product?> GetProductByIdAsync(int id);
        Task<ProductCreateViewModel> PrepareNewProductViewModelAsync();
        Task CreateProductAsync(ProductCreateViewModel viewModel);
        Task<ProductEditViewModel?> PrepareEditProductViewModelAsync(int id);
        Task UpdateProductAsync(ProductEditViewModel viewModel);
        Task DeleteProductAsync(int id);
        Task<List<ProductSearchDTO>> SearchAvailableProductsAsync(string keyword);
    }
}