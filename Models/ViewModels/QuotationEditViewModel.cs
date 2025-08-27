using System.ComponentModel.DataAnnotations;

namespace Project.Models.ViewModels
    
{
    public class QuotationEditViewModel
    {
        public int QuotationID { get; set; }

        [Required]
        public string QuotationNumber { get; set; } = null!;

        [Required(ErrorMessage = "客戶為必填項")]
        public int MemberID { get; set; }

        [Required(ErrorMessage = "員工為必填項")]
        public int EmployeeID { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public DateOnly QuoteDate { get; set; }
        public DateOnly? ValidityPeriod { get; set; }


        [Required]
        public string Status { get; set; } = null!;
        public string? Note { get; set; }

        // 使用單數的 QuotationDetail
        public List<QuotationDetailViewModel> QuotationDetail { get; set; } = new List<QuotationDetailViewModel>();
    }

    public class QuotationDetailViewModel
    {
        public int QuotationDetailID { get; set; }
        public int QuotationID { get; set; }
        public int ProductDetailID { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "價格必須大於 0")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "數量必須大於 0")]
        public int Quantity { get; set; }

        // --- 以下為顯示用，不會從表單提交 ---
        public string ProductNameAndSpec { get; set; }
    }
}