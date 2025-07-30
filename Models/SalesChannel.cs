using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class SalesChannel
{
    public int SalesChannelID { get; set; }

    public string SalesChannelName { get; set; } = null!;

    public string? Note { get; set; }

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();
}
