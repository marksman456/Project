using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectData.Models;

public partial class ProductModel
{
    
    public int ProductModelID { get; set; }
    [Display(Name = "型號名稱")]
    public string ProductModelName { get; set; } = null!;

    [Required(ErrorMessage = "請選擇分類")]

    [Display(Name = "類別")]
    public int? CategoryID { get; set; }
    [Display(Name = "品牌")]
    public string? Brand { get; set; }

    [Display(Name = "類別")]
    public virtual Category? Category { get; set; }
    public virtual ICollection<ModelSpec> ModelSpec { get; set; } = new List<ModelSpec>();

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();
}
