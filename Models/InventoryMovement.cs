using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class InventoryMovement
{
    public int MovementID { get; set; }

    public int ProductDetailID { get; set; }

    public DateTime MovementDate { get; set; }

    public string MovementType { get; set; } = null!;

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public virtual ProductDetail ProductDetail { get; set; } = null!;
}
