using OECS.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECS.Services.OrderServices
{
    public interface IOrderService
    {
        void CreateOrder(int customerID, List<OrderModel> orderModels);
        List<OrderModel> ShowCustomerOrder(int customerID);
        List<OrderModel> OrderSummary(int customerID);
        decimal GetTotalAmount(int customerID);
    }
}
