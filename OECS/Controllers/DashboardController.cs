using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        [Authorize(Roles = "1, 2, 3")]
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SideMenu()
        {
            return PartialView("Partials/_SideMenu");
        }
    }
}