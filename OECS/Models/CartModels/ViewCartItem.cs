namespace OECS.Models.CartModels
{
    public class ViewCartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int ColorID { get; set; }
        public string Color { get; set; }
        public int SizeID { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string BrandName { get; set; }
        public int OrderNo { get; set; }   //order of each item's on each customer's cart
    }
}