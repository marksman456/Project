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

    [NotMapped] // 這個標籤告訴 EF Core：不要為這個屬性在資料庫中建立欄位
    public string ProductNameWithSpec
    {
        get
        {
            // 檢查關聯的 ProductModel 和 ModelSpec 是否存在且有關聯資料
            if (ProductModel?.ModelSpec == null || !ProductModel.ModelSpec.Any())
            {
                return ProductName; // 如果沒有規格，就只回傳產品名稱
            }

            // 將所有規格值用 ", " 串接起來
            var specs = string.Join(", ", ProductModel.ModelSpec.Select(s => s.SpecValue));

            // 組合成最終的顯示名稱 (例如: "產品名稱 (規格1, 規格2)")
            return $"{ProductName} ({specs})";
        }
    }
}
