using OECS.Models;
using OECS.Repository.CustomerRepository;
using OECS.Services.CustomerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomerController()
        {
            _customerService = new CustomerService(new CustomerRepository(new oecsEntities()));
        }

        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetCustomerAddress()
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);

            return Json(new { address = _customerService.GetCustomer(customerID).address }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<Claim> GetClaim()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            return claims;
        }
    }
}