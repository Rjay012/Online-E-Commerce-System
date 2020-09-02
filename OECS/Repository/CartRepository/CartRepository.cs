using OECS.Models;
using System.Collections.Generic;
using System.Linq;

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
            return ViewCart(customerID).Where(c => c.ProductDetailID == productDetailID)
                                       .Any();
        }

        public IQueryable<Cart> ViewCart(int customerID)
        {
            return _dbContext.Cart
                             .Where(c => c.CustomerID == customerID);
        }

        public void Delete(List<Cart> carts)
        {
            foreach (var item in carts)
            {
                Cart cart = _dbContext.Cart.Find(item.CartID);
                _dbContext.Cart.Remove(cart);
                _dbContext.SaveChanges();
            }
        }
    }
}