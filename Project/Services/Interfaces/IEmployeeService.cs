using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectData.Models;
using Project.ViewModels.VMEmployee;

namespace Project.Services.Interfaces // 請確認您的命名空間
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetEmployeesForIndexAsync();
        Task<EmployeeViewModel> GetEmployeeDetailsViewModelAsync(int id);
        Task<EmployeeViewModel?> GetEmployeeEditViewModelAsync(int id);
        Task CreateEmployeeAsync(EmployeeViewModel viewModel);
        Task<bool> UpdateEmployeeAsync(EmployeeViewModel viewModel);
        Task<SelectList> GetAvailableUsersForDropdownAsync(int? currentEmployeeId = null);
        Task<bool> EmployeeExistsAsync(int id);
    }
}