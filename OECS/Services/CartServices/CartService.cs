using OECS.Models;
using OECS.Models.CartModels;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Repository.CartRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public bool AddItem(int customerID, int productDetailID)
        {
            Cart cart = new Cart()
            {
                CustomerID = customerID,
                ProductDetailID = productDetailID
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
                Cart = _cartRepository.ViewAddedItem(customerID)
                                      .Where(c => c.ProductDetail.Product.ProductID == productID)
                                      .FirstOrDefault(),
                Carts = _cartRepository.ViewAddedItem(customerID)
                                       .ToList()
            };
            return cartModel;
        }
    }
}