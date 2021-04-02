using OECS.Models;
using OECS.Models.CartModels;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Repository.CartRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        public CartService(ICartRepository cartRepository, IProductDetailRepository productDetailRepository)
        {
            _cartRepository = cartRepository;
            _productDetailRepository = productDetailRepository;
        }

        public CartService(IProductDetailRepository productDetailRepository)
        {
            _productDetailRepository = productDetailRepository;
        }

        public bool AddItem(int customerID, int productDetailID, int orderNo)
        {
            Cart cart = new Cart()
            {
                CustomerID = customerID,
                ProductDetailID = productDetailID,
                orderBy = orderNo,
                status = "on process"
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
                                      c.status,
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
                                      Status = s.Key.status,
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

        public void Checkout(int customerID, int orderNo, int quantity)
        {
            _cartRepository.Checkout(customerID, orderNo, quantity);
        }

        public void Discard(int customerID)
        {
            _cartRepository.Discard(customerID);
        }

        public void GetCheckoutItem(int customerID)  //get checkout item and set product detail status to unavailable
        {
            List<Cart> carts = _cartRepository.ViewCart(customerID)
                                              .Where(c => c.status == "checkout")
                                              .ToList();
            foreach(var item in carts)
            {
                _productDetailRepository.SetProductDetailStatus((int)item.ProductDetailID, "unavailable");
            }
        }

        public int GetProductQuantity(int productID, int colorID, int sizeID)
        {
            ViewProductDetailModel viewProductDetailModel = new ViewProductDetailModel()
            {
                ProductID = productID,
                ColorID = colorID,
                SizeID = sizeID
            };

            return _productDetailRepository.ProductDetailList(viewProductDetailModel)
                                           .Count();
        }
    }
}