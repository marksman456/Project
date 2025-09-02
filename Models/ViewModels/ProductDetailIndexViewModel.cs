namespace Project.Models.ViewModels
{
    public class ProductDetailIndexViewModel
    {
        public List<ProductDetail> SerializedItems { get; set; } = new();
        public List<ProductDetail> NonSerializedItems { get; set; } = new();

    }
}
