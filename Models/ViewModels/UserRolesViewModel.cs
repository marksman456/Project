
namespace Project.Models.ViewModels 
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; } 
        public string UserName { get; set; }
        public List<RoleSelection> Roles { get; set; } = new();
    }

    public class RoleSelection
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}