using System;

namespace OECS.Models.OrderModels.PaymentModels
{
    public class PaymentModel
    {
        public int PaymentID { get; set; }
        public int PaymentTypeID { get; set; }
        public int CustomerID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}