using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class ShippingDetail
{
    public int ShippingOrderID { get; set; }

    public int ProductDetailID { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual ShippingOrder ShippingOrder { get; set; } = null!;
}
