using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectData.Models;

public partial class ProductDetail
{
    public int ProductDetailID { get; set; }

    public int ProductID { get; set; }

    public string? SerialNumber { get; set; } = null!;

    public DateOnly? PurchaseDate { get; set; }
    [Column(TypeName = "decimal(18, 2)")]
    public decimal PurchaseCost { get; set; }

    public string Status { get; set; } = null!;

    public int Quantity { get; set; }
    public decimal Price { get; set; }


    public virtual ICollection<InventoryMovement> InventoryMovement { get; set; } = new List<InventoryMovement>();

    public virtual ICollection<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<PurchaseDetails> PurchaseDetails { get; set; } = new List<PurchaseDetails>();

    public virtual ICollection<QuotationDetail> QuotationDetail { get; set; } = new List<QuotationDetail>();

    public virtual ICollection<ShippingDetail> ShippingDetail { get; set; } = new List<ShippingDetail>();
}
