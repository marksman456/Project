using System;
using System.Collections.Generic;

namespace ProjectData.Models;

public partial class Quotation
{
    public int QuotationID { get; set; }

    public string QuotationNumber { get; set; } = null!;

    public int MemberID { get; set; }

    public int EmployeeID { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? LastUpdate { get; set; }

    public DateOnly QuoteDate { get; set; }

    public DateOnly? ValidityPeriod { get; set; }

    public string Status { get; set; } = null!;

    public bool IsTransferred { get; set; }

    public string? Note { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;

    public virtual ICollection<QuotationDetail> QuotationDetail { get; set; } = new List<QuotationDetail>();
}
