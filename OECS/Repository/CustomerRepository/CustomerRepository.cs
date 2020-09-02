using OECS.Models;

namespace OECS.Repository.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly oecsEntities _dbContext;
        public CustomerRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public Customer GetCustomer(int customerID)
        {
            return _dbContext.Customer.Find(customerID);
        }
    }
}