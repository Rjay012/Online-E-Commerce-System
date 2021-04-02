using OECS.Models.CartModels;
using System;

namespace OECS.Models.OrderModels
{
    public class OrderModel : ViewCartItem
    {
        public string OrderNumber { get; set; }
        public int CustomerID { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
    }
}