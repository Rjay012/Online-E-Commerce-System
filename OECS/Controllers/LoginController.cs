using OECS.Models;
using OECS.Models.LoginModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class LoginController : Controller
    {
        oecsEntities dbContext = new oecsEntities();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginForm()
        {
            return PartialView("Partials/_Login");
        }

        public ActionResult RegisterForm()
        {
            return PartialView("Partials/_Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel registerModel)
        {
            Customer customer = new Customer();
            if (ModelState.IsValid)
            {
                customer.fname = registerModel.Firstname;
                customer.mname = registerModel.Middlename;
                customer.lname = registerModel.Lastname;
                customer.address = registerModel.Address;
                customer.email = registerModel.Email;
                customer.phone = registerModel.Phone;
                customer.password = registerModel.Password;
                customer.RoleID = 3; //register as Customer

                dbContext.Customers.Add(customer);
                dbContext.SaveChanges();
            }
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
    }
}