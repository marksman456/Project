using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ModelSpec
{
    public int ModelSpecID { get; set; }

    public int ProductModelID { get; set; }

    public string Spec { get; set; } = null!;

    public string SpecValue { get; set; } = null!;

    public virtual ProductModel ProductModel { get; set; } = null!;
}
