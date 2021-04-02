using OECS.Models;
using OECS.Models.OrderModels;
using OECS.Models.OrderModels.PaymentModels;
using OECS.Repository.OrderRepository.PaymentRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Services.OrderServices.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public void CreatePayment(PaymentModel paymentModel, List<OrderModel> orderModels)
        {
            Payment payment = new Payment()
            {
                PaymentTypeID = paymentModel.PaymentTypeID,
                Amount = paymentModel.Amount,
                PaymentDate = paymentModel.PaymentDate,
                CustomerID = paymentModel.CustomerID,
                Status = "on process"
            };

            _paymentRepository.CreatePayment(payment, orderModels);
        }
    }
}