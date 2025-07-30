using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int MemberID { get; set; }

    public virtual Member Member { get; set; } = null!;
}
