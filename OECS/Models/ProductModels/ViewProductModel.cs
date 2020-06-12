namespace OECS.Models.ProductModels
{
    public class ViewProductModel
    {
        public Product Product { get; set; }
        public ProductImage ProductImage { get; set; }
        public Color Color { get; set; }
        public Size Size { get; set; }
        public ProductDetail ProductDetail { get; set; }
        public DisplayColor DisplayColor { get; set; }
        public DisplaySize DisplaySize { get; set; }
        public Category Category { get; set; }
        public Icon Icon { get; set; }
    }
}