using OECS.Models;
using OECS.Repository.CustomerRepository;

namespace OECS.Services.CustomerServices
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public Customer GetCustomer(int customerID)
        {
            return _customerRepository.GetCustomer(customerID);
        }
    }
}