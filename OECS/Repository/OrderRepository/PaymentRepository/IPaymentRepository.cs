using OECS.Models;
using OECS.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECS.Repository.OrderRepository.PaymentRepository
{
    public interface IPaymentRepository
    {
        void CreatePayment(Payment payment, List<OrderModel> orderModels);
    }
}
