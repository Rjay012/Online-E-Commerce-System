using OECS.Models;
using OECS.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Repository.OrderRepository.PaymentRepository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly oecsEntities _dbContext;
        public PaymentRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreatePayment(Payment payment, List<OrderModel> orderModels)
        {
            try
            {
                _dbContext.Payment.Add(payment);
                _dbContext.SaveChanges();

                if (payment.CustomerID != null)
                {
                    int paymentID = GetPaymentID((int)payment.CustomerID);
                    OrderPayment(paymentID, orderModels);
                }
            }
            catch
            {
                //error
            }
        }

        private void OrderPayment(int paymentID, List<OrderModel> orderModels)
        {
            foreach (var order in orderModels)
            {
                OrderPayment orderPayment = new OrderPayment()
                {
                    PaymentID = paymentID,
                    OrderNumber = order.OrderNumber
                };

                _dbContext.OrderPayment.Add(orderPayment);
                _dbContext.SaveChanges();
            }
        }

        private int GetPaymentID(int customerID)
        {
            return _dbContext.Payment
                             .Where(p => p.CustomerID == customerID)
                             .Max(p => p.PaymentID);
        }
    }
}