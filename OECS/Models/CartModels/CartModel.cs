using System.Collections.Generic;

namespace OECS.Models.CartModels
{
    public class CartModel
    {
        public int CartID { get; set; }
        public int CustomerID { get; set; }
        public int Quantity { get; set; }
        public Cart Cart { get; set; }
        public List<Cart> Carts { get; set; }
    }
}