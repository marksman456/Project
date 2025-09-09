using Project.ViewModels.VMOrder;
using ProjectData.Models;

namespace Project.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderIndexViewModel>> GetOrdersForIndexAsync();
        Task<Order?> GetOrderForDetailsAsync(int id);
        Task<Order> CreateOrderFromQuotationAsync(int quotationId, int paymethodId, int salesChannelId);
        Task<bool> MarkOrderAsPaidAsync(int id);
        Task<bool> MarkOrderAsShippedAsync(int id);
    }
}