using OECS.Models.CartModels;
using System.Collections.Generic;

namespace OECS.Services.CartServices
{
    public interface ICartService
    {
        bool AddItem(int customerID, int productDetailID, int orderNo);
        bool CheckDuplicateItem(int customerID, int productDetailID);
        CartModel ViewAddedItem(int productID, int customerID, int quantity);
        List<ViewCartItem> LoadCart(int customerID);
        int? CustomerCartLastOrderNo(int customerID);
        void DeleteItem(int customerID, int orderNo);
    }
}
