using System;
using System.Collections.Generic;

namespace ProjectData.Models;

public partial class PurchaseDetails
{
    public int PurchaseOrderID { get; set; }

    public int ProductDetailID { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}
