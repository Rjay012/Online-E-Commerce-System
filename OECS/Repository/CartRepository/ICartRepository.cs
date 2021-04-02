using OECS.Models;
using OECS.Models.OrderModels;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Repository.CartRepository
{
    public interface ICartRepository
    {
        bool AddItem(Cart cart);
        bool CheckDuplicateItem(int customerID, int productDetailID);
        IQueryable<Cart> ViewCart(int customerID);
        void Delete(List<Cart> carts);
        void Checkout(int customerID, int orderNo, int quantity);
        void Discard(int customerID);
    }
}
