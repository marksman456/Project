using System;
using System.Collections.Generic;

namespace ProjectData.Models;

public partial class Paymethod
{
    public int PaymethodID { get; set; }

    public string PaymethodName { get; set; } = null!;

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();
}
