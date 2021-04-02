using OECS.Models.OrderModels;
using OECS.Models.OrderModels.PaymentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECS.Services.OrderServices.PaymentServices
{
    public interface IPaymentService
    {
        void CreatePayment(PaymentModel paymentModel, List<OrderModel> orderModels);
    }
}
