using OECS.Models;
using OECS.Models.LoginModels;
using OECS.Models.ModuleModels;
using OECS.Models.RoleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                bool isExist = false;
                switch (loginModel.RoleID)
                {
                    case 1:
                        isExist = dbContext.Administrator.Where(x => x.AdminID.Trim().Equals(loginModel.UserID.Trim()) && x.password.Equals(loginModel.Password)).Any();
                        break;
                    case 2:
                        isExist = dbContext.Supplier.Where(x => x.email.Trim().Equals(loginModel.UserID.Trim()) && x.password.Equals(loginModel.Password)).Any();
                        break;
                    case 3:
                        isExist = dbContext.Customer.Where(x => x.email.Trim().Equals(loginModel.UserID.Trim()) && x.password.Equals(loginModel.Password)).Any();
                        break;
                }

                LoginModel loginCredentials = new LoginModel();
                if (isExist == true && loginModel.RoleID == 1) //logged in as administrator
                {
                    loginCredentials = dbContext.Administrator.Select(x => new LoginModel
                    {
                        UserID = x.AdminID,
                        RoleID = x.RoleID
                    }).FirstOrDefault();

                    var module = (from m in dbContext.Module
                                  join s in dbContext.SubModule on m.ModuleID equals s.ModuleID
                                  select new ViewModuleModel
                                  {
                                      Module = m,
                                      SubModule = s,
                                  }).ToList();
                    Session["Modules"] = module;
                }
                else  //logged in as customer/supplier
                {

                }

                FormsAuthentication.SetAuthCookie(loginCredentials.UserID, false);
                Session["UserID"] = loginCredentials.UserID;
                Session["LoginCredentials"] = loginCredentials;
                if (isExist == true)
                {
                    return Json(new { action = "Index", controller = "Dashboard" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("failed", JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginForm(LoginModel loginModel)
        {
            loginModel.RoleList = GetRoleListItems();
            return PartialView("Partials/_Login", loginModel);
        }

        public ActionResult RegisterForm()
        {
            return PartialView("Partials/_Register");
        }

        private IEnumerable<SelectListItem> GetRoleListItems()
        {
            List<SelectListItem> RoleListTempStorage = new List<SelectListItem>();
            var role = dbContext.Role.ToList();
            foreach (var item in role)
            {
                RoleListTempStorage.Add(new SelectListItem
                {
                    Value = item.RoleID.ToString(),
                    Text = item.Role1
                });
            }

            return RoleListTempStorage;
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

                dbContext.Customer.Add(customer);
                dbContext.SaveChanges();
            }
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
    }
}