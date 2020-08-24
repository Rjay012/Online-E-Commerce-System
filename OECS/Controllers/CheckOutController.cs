using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CheckOutController : Controller
    {
        [Authorize(Roles = "3")]
        // GET: CheckOut
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "3")]
        public ActionResult LoadPackage()
        {
            return PartialView("Partials/Tables/_Package");
        }

        [Authorize(Roles = "3")]
        public ActionResult ShippingAndOrderSummary()
        {
            return PartialView("Partials/Cards/_ShippingAndOrderSummary");
        }

        [Authorize(Roles = "3")]
        public ActionResult PaymentMethodConfirmationModal()
        {
            return PartialView("Partials/Modals/_PaymentMethodConfirmation");
        }
    }
}