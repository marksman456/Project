using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectData.Models;

public partial class Product
{
    public int ProductID { get; set; }

    public string ProductSKU { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public int ProductModelID { get; set; }

    public decimal Price { get; set; }

    public string? ProductImage { get; set; }

    public bool IsSerialized { get; set; }

    public virtual ICollection<ProductDetail> ProductDetail { get; set; } = new List<ProductDetail>();

    public virtual ProductModel ProductModel { get; set; } = null!;
    //一個具體的商品，是由多個明確的規格所定義的。
    public virtual ICollection<ModelSpec> Specs { get; set; } = new List<ModelSpec>();

    [NotMapped]
    public string ProductNameWithSpec
    {
        get
        {
            // 【核心修改】直接讀取已關聯的 Specs 集合，不再需要做任何文字比對
            if (Specs == null || !Specs.Any())
            {
                return ProductName;
            }

            var specsText = string.Join(", ", Specs.Select(s => s.SpecValue));
            return $"{ProductName} ({specsText})";
        }
    }
}
