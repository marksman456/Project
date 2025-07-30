using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ProductModel
{
    public int ProductModelID { get; set; }

    public string ProductModelName { get; set; } = null!;

    public int CategoryID { get; set; }

    public string? Brand { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<ModelSpec> ModelSpec { get; set; } = new List<ModelSpec>();

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();
}
