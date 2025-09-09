using ProjectData.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectData.Models;

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

    [NotMapped]
    public string GenderDisplay =>
      Gender switch
      {
          "M" => "男",
          "F" => "女",
          "O" => "其他",
          _ => "未指定"
      };
    public string? Note { get; set; }

    public virtual ICollection<Account> Account { get; set; } = new List<Account>();

    public virtual ICollection<Order> Order { get; set; } = new List<Order>();

    public virtual ICollection<Quotation> Quotation { get; set; } = new List<Quotation>();
}
