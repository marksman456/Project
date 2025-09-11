using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectData.Models;

public partial class Category
{
    public int CategoryID { get; set; }
    [Display(Name = "類別名稱")]
    public string CategoryName { get; set; } = null!;
    [Display(Name = "描述")]
    public string? Description { get; set; }
    [Display(Name = "類別圖片")]
    public string? CategoryImage { get; set; }
    [Display(Name = "狀態")]
    public string Status { get; set; } = null!;
    [Display(Name = "創建時間")]


    public DateTime CreatedDate { get; set; }

    public virtual ICollection<ProductModel> ProductModel { get; set; } = new List<ProductModel>();
}
