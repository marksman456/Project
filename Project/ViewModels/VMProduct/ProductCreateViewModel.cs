using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Project.ViewModels.VMProduct
{
    // 備註：這個 ViewModel 用於「新增產品」的表單，包含了所有需要使用者填寫的欄位。
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "產品SKU為必填項")]
        [Display(Name = "產品SKU")]
        public string ProductSKU { get; set; } = null!;

        [Required(ErrorMessage = "產品名稱為必填項")]
        [Display(Name = "產品名稱")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "描述")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "必須選擇一個產品型號")]
        [Display(Name = "產品型號")]
        public int ProductModelID { get; set; }

        [Required(ErrorMessage = "售價為必填項")]
        [Range(0, double.MaxValue, ErrorMessage = "售價不能為負數")]
        [Display(Name = "官方售價")]
        public decimal Price { get; set; }


        [Display(Name = "是否為序號化商品")]
        public bool IsSerialized { get; set; }

        // 備註：專門用來接收上傳的圖片檔案。
        [Display(Name = "產品圖片")]
        public IFormFile? ProductImageFile { get; set; }

        // 備註：用於在 View 中產生「產品型號」的下拉選單。
        public SelectList? ProductModelList { get; set; }

        public List<int> SelectedSpecIds { get; set; } = new();
    }
}