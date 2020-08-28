using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Repository.CartRepository
{
    public class CartRepository : ICartRepository
    {
        private readonly oecsEntities _dbContext;

        public CartRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AddItem(Cart cart)
        {
            try
            {
                _dbContext.Cart.Add(cart);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckDuplicateItem(int customerID, int productDetailID)
        {
            return _dbContext.Cart
                             .Where(c => c.CustomerID == customerID && c.ProductDetailID == productDetailID)
                             .Any();
        }

        public IQueryable<Cart> ViewAddedItem(int customerID)
        {
            return _dbContext.Cart
                             .Where(c => c.CustomerID == customerID);
        }
    }
}