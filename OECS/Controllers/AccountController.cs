using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OECS.Models;
using OECS.Models.LoginModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class AccountController : Controller
    {
        oecsEntities dbContext = new oecsEntities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        #region("USER LOGIN")
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            if (loginModel.UserID == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                string userID = "";
                bool isExist = false, isEmail = false;

                switch (loginModel.RoleID)
                {
                    case 1:
                        isExist = dbContext.Administrator.Where(x => x.AdminID.Trim().Equals(loginModel.UserID.Trim()) && x.password.Equals(loginModel.Password)).Any();
                        break;
                    case 2:
                        isExist = dbContext.Supplier.Where(x => x.email.Trim().Equals(loginModel.UserID.Trim()) && x.password.Equals(loginModel.Password)).Any();
                        isEmail = true;
                        break;
                    case 3:
                        isExist = GetCustomer(loginModel).Any();
                        isEmail = true;
                        userID = GetCustomer(loginModel).Select(s => new { s.CustomerID })
                                                        .FirstOrDefault()
                                                        .CustomerID
                                                        .ToString();
                        break;
                }

                if (isExist == true)
                {
                    List<ViewModuleModel> module = (from m in dbContext.Module
                                                    join r in dbContext.RoleModule on m.ModuleID equals r.ModuleID
                                                    join s in dbContext.SubModule on m.ModuleID equals s.ModuleID into g
                                                    from sub in g.DefaultIfEmpty()  //left join
                                                    select new ViewModuleModel
                                                    {
                                                        Module = m,
                                                        SubModule = sub ?? null,
                                                        RoleModule = r
                                                    }).Where(r => r.RoleModule.RoleID == loginModel.RoleID).ToList();
                    Session["Modules"] = module;  //store modules
                    userID = userID == "" ? loginModel.UserID : userID;

                    //user login credentials
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.Sid, userID));
                    claims.Add(new Claim(ClaimTypes.Role, loginModel.RoleID.ToString()));
                    if (isEmail == true)  //store the email of customer/supplier
                    {
                        claims.Add(new Claim(ClaimTypes.Email, loginModel.UserID));
                    }

                    var claimIdentities = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimIdentities);

                    if (loginModel.ShopNow == true && loginModel.RoleID == 3)
                    {
                        Session["ShopNow"] = loginModel.ShopNow;  //activate session for customer shopping
                        return Json(new { data = "success", shopNow = true }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { controller = "Dashboard" }, JsonRequestBehavior.AllowGet);  //bypassed main target module
                }
            }
            return View();
        }

        private IQueryable<Customer> GetCustomer(LoginModel loginModel)
        {
            return dbContext.Customer.Where(x => x.email.Trim().Equals(loginModel.UserID.Trim()) && x.password.Equals(loginModel.Password));
        }

        public ActionResult LoginForm(LoginModel loginModel)
        {
            loginModel.RoleList = GetRoleListItems();
            return PartialView("Partials/_Login", loginModel);
        }
        #endregion("/USER LOGIN")

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

        #region("REGISTER")
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

                dbContext.Customer.Add(customer);
                dbContext.SaveChanges();
            }
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        #endregion("/REGISTER")

        [HttpPost]
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            Session["Modules"] = null;
            Session["ShopNow"] = null;
            authenticationManager.SignOut();

            return RedirectToAction("Index", "Account");
        }
    }
}