using OECS.Models;
using OECS.Models.OrderModels;
using OECS.Repository.OrderRepository;
using OECS.Services.CartServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public void CreateOrder(int customerID, List<OrderModel> orderModels)
        {
            foreach (var item in orderModels)
            {
                Order order = new Order()
                {
                    OrderNumber = item.OrderNumber,
                    CustomerID = customerID,
                    ProductID = item.ProductID,
                    OrderDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                    ShippingAddress = item.ShippingAddress,
                    Status = "on process",
                };

                if (_orderRepository.CreateOrder(order) == true)
                {
                    //create order detail
                    OrderDetail orderDetail = CreateOrderDetail(new OrderModel() { OrderNumber = item.OrderNumber, Quantity = item.Quantity, Price = item.Price });
                    _orderRepository.CreateOrderDetail(orderDetail);
                }
            }
        }

        private OrderDetail CreateOrderDetail(OrderModel orderModel)
        {
            OrderDetail orderDetail = new OrderDetail()
            {
                OrderNumber = orderModel.OrderNumber,
                Quantity = orderModel.Quantity,
                Price = orderModel.Price
            };

            return orderDetail;
        }

        public List<OrderModel> ShowCustomerOrder(int customerID)
        {
            return _orderRepository.ShowCustomerOrder(customerID);
        }

        public List<OrderModel> OrderSummary(int customerID)
        {
           return _orderRepository.OrderSummary(customerID);
        }

        public decimal GetTotalAmount(int customerID)
        {
            List<OrderModel> orderModels = _orderRepository.OrderSummary(customerID);

            decimal totalAmount = 0.0m;
            foreach(var item in orderModels)
            {
                totalAmount += item.Price * item.Quantity;
            }

            return totalAmount + (totalAmount * (decimal)0.12);
        }
    }
}