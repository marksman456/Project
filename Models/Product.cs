using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Product
{
    public int ProductID { get; set; }

    public string ProductSKU { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public int ProductModelID { get; set; }

    public decimal Price { get; set; }

    public string? ProductImage { get; set; }

    public virtual ICollection<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();

    public virtual ProductModel ProductModel { get; set; } = null!;
}
