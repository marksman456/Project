using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Project.ViewModels.Quotation
{
    public class QuotationViewModel : IValidatableObject
    {

        public int QuotationID { get; set; }


        [Required(ErrorMessage = "必填")]
        public string? QuotationNumber { get; set; }

        [Display(Name = "客戶")]
     
        [Required(ErrorMessage = "客戶為必填項")]
        public int MemberID { get; set; }

        [Display(Name = "員工")]
        [Required(ErrorMessage = "員工為必填項")]
        public int EmployeeID { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastUpdate { get; set; }



       
        [Display(Name = "報價日期")]
        [Required(ErrorMessage = "報價日期為必填項")]
        public DateOnly QuoteDate { get; set; }

        // 有效期限本來就是可為空的，所以維持不變
        [Display(Name = "有效期限")]
        public DateOnly? ValidityPeriod { get; set; }
        [Display(Name = "狀態")]
        public string Status { get; set; } = null!;
        public string? Note { get; set; }


        public IEnumerable<SelectListItem> MemberList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> EmployeeList { get; set; } = new List<SelectListItem>();
        public List<QuotationDetailViewModel> QuotationDetail { get; set; } = new List<QuotationDetailViewModel>();




        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ValidityPeriod.HasValue && ValidityPeriod.Value < QuoteDate)
            {
                yield return new ValidationResult(
                    "有效期限不能小於報價日期。",
                    new[] { nameof(ValidityPeriod) }
                );
            }
            if (!QuotationDetail.Any())
            {
                yield return new ValidationResult("報價單至少需要一個品項。", new[] { nameof(QuotationDetail) });
            }
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


            [Range(0, 1, ErrorMessage = "折扣必須介於 0 和 1 之間")]
            public decimal? Discount { get; set; }
            public string? ProductNameAndSpec { get; set; }
        }



    }
}

