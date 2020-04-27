using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OECS.Models;
using OECS.Models.LoginModels;
using OECS.Models.ModuleModels;
using OECS.Models.RoleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel loginModel)
        {
            if(loginModel.UserID == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                List<Claim> claims  = new List<Claim>();
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

                    //user login credentials
                    claims.Add(new Claim(ClaimTypes.Name, loginModel.UserID));
                    claims.Add(new Claim(ClaimTypes.Role, loginModel.RoleID.ToString()));
                    var claimIdentities = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, claimIdentities);

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