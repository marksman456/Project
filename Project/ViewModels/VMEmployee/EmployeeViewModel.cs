using System.ComponentModel.DataAnnotations;

namespace Project.ViewModels.VMEmployee
{
    public class EmployeeViewModel
    {
        public int EmployeeID { get; set; }


        [Display(Name = "員工編號")]
        [Required(ErrorMessage = "員工編號為必填項。")]
        [StringLength(20)]
        public string EmployeeNumber { get; set; } = null!;

        [Display(Name = "是否離職")]
        public bool Resignation { get; set; }

        [Display(Name = "職稱")]
        public string? Title { get; set; }

        [Display(Name = "到職日")]
        public DateOnly? HireDate { get; set; }

        [Display(Name = "手機")]
        public string? Phone { get; set; }

        [Display(Name = "地址")]
        public string? Address { get; set; }

        [Display(Name = "信箱")]
        public string? Email { get; set; }

        [Display(Name = "性別")]
        public string? Gender { get; set; }

        [Display(Name = "生日")]
        public DateOnly? Birthday { get; set; }

        [Display(Name = "備註")]
        public string? Note { get; set; }
        [Required(ErrorMessage = "員工姓名為必填欄位。")]
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; } = null!;

        [Display(Name = "關聯系統帳號 (登入用)")]
        public string? UserId { get; set; }
    }
}