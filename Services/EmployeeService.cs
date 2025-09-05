using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Services.Interfaces;
using Project.ViewModels;
using Project.ViewModels.Employee;
using ProjectData.Data;
using ProjectData.Models;





namespace Project.Services 
{
    public class EmployeeService : IEmployeeService
    {
        private readonly XiangYunDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeeService(XiangYunDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesForIndexAsync()
        {
            return await _context.Employee.Include(e => e.User).ToListAsync();
        }

        public async Task<EmployeeViewModel> GetEmployeeDetailsViewModelAsync(int id)
        {
            var employee = await _context.Employee
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);

            if (employee == null)
            {
                // 在真實世界中，這裡可以拋出一個自訂的 NotFoundException
                // 為了簡單起見，我們先回傳 null，由 Controller 處理
                return null;
            }

            return new EmployeeViewModel
            {
                EmployeeID = employee.EmployeeID,
                EmployeeNumber = employee.EmployeeNumber,
                EmployeeName = employee.Name,
                Phone = employee.Phone,
                Address = employee.Address,
                Email = employee.Email,
                Gender = employee.Gender,
                Birthday = employee.Birthday,
                Title = employee.Title,
                HireDate = employee.HireDate,
                Resignation = employee.Resignation,
                Note = employee.Note,
                // 如果 employee.User 不是 null，就取得它的 UserName，否則就顯示 "未指派"
                UserId = employee.User?.UserName ?? "未指派"
            };
        }

        public async Task<EmployeeViewModel?> GetEmployeeEditViewModelAsync(int id)
        {
            var employee = await _context.Employee.FindAsync(id);

            if (employee == null)
            {
                return null;
            }

            return new EmployeeViewModel
            {
                EmployeeID = employee.EmployeeID,
                EmployeeNumber = employee.EmployeeNumber,
                Resignation = employee.Resignation,
                Phone = employee.Phone,
                Address = employee.Address,
                Gender= employee.Gender,
                Email = employee.Email,
                Birthday = employee.Birthday,
                Note = employee.Note,
                EmployeeName = employee.Name,
                Title = employee.Title,
                HireDate = employee.HireDate,
                UserId = employee.UserId
            };
        }

        public async Task CreateEmployeeAsync(EmployeeViewModel viewModel)
        {
            var employee = new Employee
            {
                EmployeeNumber = viewModel.EmployeeNumber,
                Resignation = viewModel.Resignation,
                Phone = viewModel.Phone,
                Address = viewModel.Address,
                Email = viewModel.Email,
                Gender = viewModel.Gender,
                Birthday = viewModel.Birthday,
                Note = viewModel.Note,
                Name = viewModel.EmployeeName,
                Title = viewModel.Title,
                HireDate = viewModel.HireDate,
                UserId = viewModel.UserId
            };
            _context.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeViewModel viewModel)
        {
            var employeeToUpdate = await _context.Employee.FindAsync(viewModel.EmployeeID);
            if (employeeToUpdate == null)
            {
                return false; // 更新失敗
            }

            employeeToUpdate.EmployeeNumber = viewModel.EmployeeNumber;
            employeeToUpdate.Resignation = viewModel.Resignation;
            employeeToUpdate.Phone = viewModel.Phone;
            employeeToUpdate.Address = viewModel.Address;
            employeeToUpdate.Email = viewModel.Email;
            employeeToUpdate.Gender = viewModel.Gender;
            employeeToUpdate.Birthday = viewModel.Birthday;
            employeeToUpdate.Note = viewModel.Note;
            employeeToUpdate.Name = viewModel.EmployeeName;
            employeeToUpdate.Title = viewModel.Title;
            employeeToUpdate.HireDate = viewModel.HireDate;
            employeeToUpdate.UserId = viewModel.UserId;

            _context.Update(employeeToUpdate);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EmployeeExistsAsync(viewModel.EmployeeID))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true; // 更新成功
        }

        public async Task<SelectList> GetAvailableUsersForDropdownAsync(int? currentEmployeeId = null)
        {
            var query = _context.Employee.Where(e => e.UserId != null);
            if (currentEmployeeId.HasValue)
            {
                query = query.Where(e => e.EmployeeID != currentEmployeeId.Value);
            }
            var assignedUserIds = await query.Select(e => e.UserId).ToListAsync();

            var availableUsers = await _userManager.Users
                .Where(u => !assignedUserIds.Contains(u.Id))
                .ToListAsync();

            return new SelectList(availableUsers, "Id", "UserName");
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await _context.Employee.AnyAsync(e => e.EmployeeID == id);
        }
    }
}