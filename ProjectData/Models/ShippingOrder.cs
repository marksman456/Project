using System;
using System.Collections.Generic;

namespace ProjectData.Models;

public partial class ShippingOrder
{
    public int ShippingOrderID { get; set; }

    public string ShippingNumber { get; set; } = null!;

    public int OrderID { get; set; }

    public DateTime ShippingDate { get; set; }

    public int EmployeeID { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<ShippingDetail> ShippingDetail { get; set; } = new List<ShippingDetail>();
}
