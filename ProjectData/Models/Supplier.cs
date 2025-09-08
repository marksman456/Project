using System;
using System.Collections.Generic;

namespace ProjectData.Models;

public partial class Supplier
{
    public int SupplierID { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? SupplierTel { get; set; }

    public string? SupplierAddress { get; set; }

    public virtual ICollection<PurchaseOrder> PurchaseOrder { get; set; } = new List<PurchaseOrder>();
}
