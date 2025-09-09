using Project.ViewModels.VMDashboard;

namespace Project.Services.Interfaces // 請確認您的命名空間
    {
        public interface IDashboardService
        {
            Task<DashboardViewModel> GetDashboardViewModelAsync();


        }
   }

