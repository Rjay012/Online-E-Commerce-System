using OECS.Models;
using OECS.Models.OrderModels;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Repository.OrderRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly oecsEntities _dbContext;
        public OrderRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public bool CreateOrder(Order order)
        {
            try
            {
                _dbContext.Order.Add(order);
                _dbContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CreateOrderDetail(OrderDetail orderDetail)
        {
            _dbContext.OrderDetail.Add(orderDetail);
            _dbContext.SaveChanges();
        }

        public List<OrderModel> ShowCustomerOrder(int customerID)
        {
            return _dbContext.CustomerOrder(customerID)
                             .Select(s => new OrderModel
                             {
                                 OrderNumber = s.OrderNumber,
                                 ProductID = s.ProductID,
                                 ProductName = s.productName,
                                 BrandName = s.BrandName,
                                 Color = s.color,
                                 ColorID = s.ColorID,
                                 Size = s.size,
                                 Price = (decimal)s.Price,
                                 Quantity = (int)s.Quantity
                             }).ToList();
        }

        public List<OrderModel> OrderSummary(int customerID)
        {
            return _dbContext.OrderDetail
                             .Where(o => o.Order.CustomerID == customerID && o.Order.Status == "on process")
                             .Select(s => new OrderModel
                             {
                                 Quantity = (int)s.Quantity,
                                 Price = (decimal)s.Price
                             }).ToList();
        }
    }
}