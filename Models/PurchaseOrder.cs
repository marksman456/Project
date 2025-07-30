using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class PurchaseOrder
{
    public int PurchaseOrderID { get; set; }

    public string PurchaseOrderNumber { get; set; } = null!;

    public int SupplierID { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Note { get; set; }

    public virtual ICollection<PurchaseDetails> PurchaseDetails { get; set; } = new List<PurchaseDetails>();

    public virtual Supplier Supplier { get; set; } = null!;
}
