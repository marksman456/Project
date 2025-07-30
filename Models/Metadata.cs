using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

// 建議將所有 Metadata 放在同一個命名空間下，請確認這是否為你的專案命名空間
namespace Project.Models
{

    //======================== Member Metadata ========================
    [ModelMetadataType(typeof(MemberMetadata))]
    public partial class Member { }

    public class MemberMetadata
    {
        [Display(Name = "會員編號")]
        [Required(ErrorMessage = "會員編號為必填項。")]
        [StringLength(20)]
        public string MemberNumber { get; set; } = null!;

        [Display(Name = "會員姓名")]
        [Required(ErrorMessage = "會員姓名為必填項。")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Display(Name = "聯絡電話")]
        [Phone(ErrorMessage = "請輸入有效的電話號碼。")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Display(Name = "聯絡地址")]
        [StringLength(100)]
        public string? Address { get; set; }

        [Display(Name = "電子郵件")]
        [EmailAddress(ErrorMessage = "請輸入有效的電子郵件地址。")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }
    }

    //======================== Employee Metadata ========================
    [ModelMetadataType(typeof(EmployeeMetadata))]
    public partial class Employee { }

    public class EmployeeMetadata
    {
        [Display(Name = "員工編號")]
        [Required(ErrorMessage = "員工編號為必填項。")]
        [StringLength(20)]
        public string EmployeeNumber { get; set; } = null!;

        [Display(Name = "員工姓名")]
        [Required(ErrorMessage = "員工姓名為必填項。")]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Display(Name = "到職日期")]
        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        [Display(Name = "是否離職")]
        public bool Resignation { get; set; }
    }

    //======================== Product Metadata ========================
    [ModelMetadataType(typeof(ProductMetadata))]
    
    partial class Product { }

    public class ProductMetadata
    {
        [Display(Name = "商品料號(SKU)")]
        [Required(ErrorMessage = "商品料號為必填項。")]
        [StringLength(50)]
        public string ProductSKU { get; set; } = null!;

        [Display(Name = "商品名稱")]
        [Required(ErrorMessage = "商品名稱為必填項。")]
        [StringLength(100)]
        public string ProductName { get; set; } = null!;


        [Display(Name = "商品描述")]
        public string? Description { get; set; }

        [Display(Name = "商品圖片")]
        public string? ProductImage { get; set; }

        [Display(Name = "商品型號")]
        [Required]
        public int ProductModelID { get; set; }

        [Display(Name = "售價")]
        [Required(ErrorMessage = "售價為必填項。")]
        [DataType(DataType.Currency)]
        [Range(0, 9999999, ErrorMessage = "價格不能為負數。")]
        public decimal Price { get; set; }
    }

    //======================== Order Metadata ========================
    [ModelMetadataType(typeof(OrderMetadata))]
    public partial class Order { }

    public class OrderMetadata
    {
        [Display(Name = "訂單編號")]
        [Required]
        public string OrderNumber { get; set; } = null!;

        [Display(Name = "訂購會員")]
        [Required]
        public int MemberId { get; set; }

        [Display(Name = "負責員工")]
        [Required]
        public int EmployeeId { get; set; }

        [Display(Name = "訂單日期")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; }
    }

    //======================== Quotation Metadata ========================
    [ModelMetadataType(typeof(QuotationMetadata))]
    public partial class Quotation { }

    public class QuotationMetadata
    {
        [Display(Name = "報價單號")]
        [Required]
        public string QuotationNumber { get; set; } = null!;

        [Display(Name = "客戶")]
        [Required]
        public int MemberId { get; set; }

        [Display(Name = "報價員工")]
        [Required]
        public int EmployeeId { get; set; }

        [Display(Name = "報價日期")]
        [DataType(DataType.Date)]
        public DateTime QuoteDate { get; set; }

        [Display(Name = "狀態")]
        [Required]
        public string Status { get; set; } = null!;
    }

    //======================== PurchaseOrder Metadata ========================
    [ModelMetadataType(typeof(PurchaseOrderMetadata))]
    public partial class PurchaseOrder { }

    public class PurchaseOrderMetadata
    {
        [Display(Name = "進貨單號")]
        [Required]
        public string PurchaseOrderNumber { get; set; } = null!;

        [Display(Name = "供應商")]
        [Required]
        public int SupplierId { get; set; }

        [Display(Name = "預計到貨日")]
        [DataType(DataType.Date)]
        public DateTime? ArrivalDate { get; set; }

        [Display(Name = "狀態")]
        [Required]
        public string Status { get; set; } = null!;
    }
}