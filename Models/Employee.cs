using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Employee
{
    public int EmployeeID { get; set; }

    public string EmployeeNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? Gender { get; set; }

    public DateOnly? Birthday { get; set; }

    public DateOnly? HireDate { get; set; }

    public bool Resignation { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();

    public virtual ICollection<Quotation> Quotation { get; set; } = new List<Quotation>();

    public virtual ICollection<ShippingOrder> ShippingOrder { get; set; } = new List<ShippingOrder>();

    public virtual ICollection<Role> Role { get; set; } = new List<Role>();
}
