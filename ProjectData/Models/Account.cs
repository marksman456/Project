using ProjectData.Models;
using System;
using System.Collections.Generic;

namespace ProjectData.Models;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int MemberID { get; set; }

    public virtual Member Member { get; set; } = null!;
}
