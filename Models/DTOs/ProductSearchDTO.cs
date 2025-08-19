namespace Project.Models.DTOs
{
    public class ProductSearchDTO
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
