using OECS.Models;
using OECS.Models.CartModels;
using OECS.Repository.CartRepository;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public bool AddItem(int customerID, int productDetailID, int orderNo)
        {
            Cart cart = new Cart()
            {
                CustomerID = customerID,
                ProductDetailID = productDetailID,
                orderBy = orderNo
            };

            return _cartRepository.AddItem(cart);
        }

        public bool CheckDuplicateItem(int customerID, int productDetailID)
        {
            return _cartRepository.CheckDuplicateItem(customerID, productDetailID);
        }

        public CartModel ViewAddedItem(int productID, int customerID, int quantity)
        {
            CartModel cartModel = new CartModel()
            {
                CustomerID = customerID,
                Quantity = quantity,
                Cart = _cartRepository.ViewCart(customerID)
                                      .Where(c => c.ProductDetail.Product.ProductID == productID)
                                      .FirstOrDefault(),
                Carts = _cartRepository.ViewCart(customerID)
                                       .ToList()
            };
            return cartModel;
        }

        public List<ViewCartItem> LoadCart(int customerID)
        {
            return _cartRepository.ViewCart(customerID)
                                  .GroupBy(c => new 
                                  { 
                                      c.ProductDetail.Product.ProductID,
                                      c.ProductDetail.Product.productName,
                                      c.ProductDetail.Color.ColorID,
                                      c.ProductDetail.Color.color1,
                                      c.ProductDetail.Size.SideID,
                                      c.ProductDetail.Size.size1,
                                      c.ProductDetail.Product.price,
                                      c.ProductDetail.Product.Brand.BrandName,
                                      c.orderBy
                                  })
                                  .Select(s => new ViewCartItem
                                  {
                                      ProductID = s.Key.ProductID,
                                      ProductName = s.Key.productName,
                                      ColorID = s.Key.ColorID,
                                      Color = s.Key.color1,
                                      SizeID = s.Key.SideID,
                                      Size = s.Key.size1,
                                      Price = (decimal)s.Key.price,
                                      BrandName = s.Key.BrandName,
                                      OrderNo = (int)s.Key.orderBy,
                                      Quantity = s.Distinct().Count()
                                  }).ToList();
        }

        public int? CustomerCartLastOrderNo(int customerID)   //get the last order no of each customer item in cart
        {
            var orderNo = _cartRepository.ViewCart(customerID)
                                         .AsEnumerable()                //convert IQueryable to Enumerable to be able to use Select
                                         .Select(s => new { s.orderBy })
                                         .ToList();

            return orderNo.Count() == 0 ? 0 : orderNo.LastOrDefault().orderBy;
        }

        public void DeleteItem(int customerID, int orderNo)
        {
            List<Cart> carts = _cartRepository.ViewCart(customerID)
                                              .Where(c => c.orderBy == orderNo)
                                              .ToList();
            _cartRepository.Delete(carts);
        }
    }
}