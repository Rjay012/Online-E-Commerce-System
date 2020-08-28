using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECS.Repository.CartRepository
{
    public interface ICartRepository
    {
        bool AddItem(Cart cart);
        bool CheckDuplicateItem(int customerID, int productDetailID);
        IQueryable<Cart> ViewAddedItem(int customerID);
    }
}
