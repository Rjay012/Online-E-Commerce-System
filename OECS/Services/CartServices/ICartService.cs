using OECS.Models;
using OECS.Models.CartModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECS.Services.CartServices
{
    public interface ICartService
    {
        bool AddItem(int customerID, int productDetailID);
        bool CheckDuplicateItem(int customerID, int productDetailID);
        CartModel ViewAddedItem(int productID, int customerID, int quantity);
    }
}
