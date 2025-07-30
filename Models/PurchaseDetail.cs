using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class PurchaseDetail
{
    public int PurchaseOrderId { get; set; }

    public int ProductDetailId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}
