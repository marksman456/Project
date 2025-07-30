using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Member
{
    public int MemberID { get; set; }

    public string MemberNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Gender { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<Account> Account { get; set; } = new List<Account>();

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();

    public virtual ICollection<Quotation> Quotation { get; set; } = new List<Quotation>();
}
