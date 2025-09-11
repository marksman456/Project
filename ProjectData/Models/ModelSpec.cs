using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectData.Models;

public partial class ModelSpec
{
    public int ModelSpecID { get; set; }

    [Display(Name = "產品型號")]
    public int ProductModelID { get; set; }
    [Display(Name = "規格")]
    public string Spec { get; set; } = null!;
    [Display(Name = "規格值")]
    public string SpecValue { get; set; } = null!;
    [Display(Name = "產品型號")]
    public virtual ProductModel? ProductModel { get; set; }
}
