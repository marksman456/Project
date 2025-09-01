using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class OrderDetail
{
    public int OrderID { get; set; }

    public int ProductDetailID { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
