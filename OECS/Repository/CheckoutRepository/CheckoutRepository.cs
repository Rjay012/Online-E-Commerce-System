using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Repository.CheckoutRepository
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly oecsEntities _dbContext;
        public CheckoutRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public bool CheckAvailableOrderNumber(string orderNumber)
        {
            return _dbContext.Order
                             .Where(o => o.OrderNumber == orderNumber)
                             .Any();
        }
    }
}