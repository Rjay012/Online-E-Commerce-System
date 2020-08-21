using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CheckOutController : Controller
    {
        // GET: CheckOut
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadPackage()
        {
            return PartialView("Partials/Tables/_Package");
        }

        public ActionResult ShippingAndOrderSummary()
        {
            return PartialView("Partials/Cards/_ShippingAndOrderSummary");
        }
    }
}