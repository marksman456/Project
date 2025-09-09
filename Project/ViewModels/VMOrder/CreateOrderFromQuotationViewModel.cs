using System.ComponentModel.DataAnnotations;

namespace Project.ViewModels.VMOrder
{
    public class CreateOrderFromQuotationViewModel
    {
        [Required]
        public int QuotationID { get; set; }

        [Required(ErrorMessage = "請選擇付款方式")]
        public int PaymethodID { get; set; }

        [Required(ErrorMessage = "請選擇銷售管道")]
        public int SalesChannelID { get; set; }
    }
}