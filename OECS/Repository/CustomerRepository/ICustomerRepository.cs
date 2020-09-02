using OECS.Models;

namespace OECS.Repository.CustomerRepository
{
    public interface ICustomerRepository
    {
        Customer GetCustomer(int customerID);
    }
}
