using System.ComponentModel.DataAnnotations;

namespace Project.ViewModels.VMProduct
{
    // 備註：編輯的 ViewModel 繼承自新增版，再加上 ProductID 和現有圖片路徑即可。
    public class ProductEditViewModel : ProductCreateViewModel
    {
        public int ProductID { get; set; }

        [Display(Name = "現有圖片")]
        public string? ExistingImagePath { get; set; }

    }
}