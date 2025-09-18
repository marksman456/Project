using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System;

namespace Project.ViewModels.VMProductDetail
{
    public class ProductDetailCreateViewModel
    {
        [Required(ErrorMessage = "請選擇一個產品")]
        [Display(Name = "產品")]
        public int ProductID { get; set; }

        [Display(Name = "產品序號 (序號化商品必填)")]
        public string? SerialNumber { get; set; }

        [Display(Name = "進貨日期")]
        public DateOnly? PurchaseDate { get; set; }

        [Required(ErrorMessage = "請輸入進貨成本")]
        [Range(0.01, double.MaxValue, ErrorMessage = "成本必須大於 0")]
        [Display(Name = "進貨成本")]
        public decimal PurchaseCost { get; set; }

        [Required(ErrorMessage = "請輸入數量")]
        [Range(1, int.MaxValue, ErrorMessage = "數量必須大於 0")]
        [Display(Name = "數量")]
        public int Quantity { get; set; }

        [Display(Name = "預計售價")]
        [Required(ErrorMessage = "請輸入預計售價")]
        public decimal SalePrice { get; set; } 

        // 用於提供給 View 建立下拉選單
        public SelectList? ProductList { get; set; }

       

    }
}