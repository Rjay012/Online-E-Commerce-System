using OECS.Models;
using OECS.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECS.Repository.OrderRepository
{
    public interface IOrderRepository
    {
        bool CreateOrder(Order order);
        void CreateOrderDetail(OrderDetail orderDetail);
        List<OrderModel> ShowCustomerOrder(int customerID);
        List<OrderModel> OrderSummary(int customerID);
    }
}
