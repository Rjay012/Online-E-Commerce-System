using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OECS.Services.CustomerServices
{
    public interface ICustomerService
    {
        Customer GetCustomer(int customerID);
    }
}
