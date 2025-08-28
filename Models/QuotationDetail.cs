using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class QuotationDetail
{
    public int QuotationID { get; set; }

    public int ProductDetailID { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? Discount { get; set; }
    public virtual ProductDetail ProductDetail { get; set; } = null!;

    public virtual Quotation Quotation { get; set; } = null!;
}
