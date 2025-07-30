using System;
using System.Collections.Generic;

namespace Project.Models;

public partial class Category
{
    public int CategoryID { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public string? CategoryImage { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<ProductModel> ProductModel { get; set; } = new List<ProductModel>();
}
