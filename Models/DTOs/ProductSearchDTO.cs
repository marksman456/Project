namespace Project.Models.DTOs
{
    public class ProductSearchDTO
    {
        public int ProductDetailId { get; set; }
        public string Label { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
