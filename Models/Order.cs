using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Order
{
    public int OrderID { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int MemberID { get; set; }

    public int EmployeeID { get; set; }

    public DateTime OrderDate { get; set; }

    public DateOnly? DeliveryDate { get; set; }

    public DateOnly? ArrivalDate { get; set; }

    public bool IsPaid { get; set; }

    public int PaymethodID { get; set; }

    public int SalesChannelID { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();

    public virtual Paymethod Paymethod { get; set; } = null!;

    public virtual SalesChannel SalesChannel { get; set; } = null!;

    public virtual ICollection<ShippingOrder> ShippingOrder { get; set; } = new List<ShippingOrder>();
}
