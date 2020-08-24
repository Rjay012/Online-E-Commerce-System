using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class OrderPurchasedController : Controller
    {
        [Authorize(Roles = "3")]
        // GET: OrderPurchased
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "3")]
        public ActionResult PurchasedGreetingCard()
        {
            return PartialView("Partials/Cards/_PurchasedGreetings");
        }

        [Authorize(Roles = "3")]
        public ActionResult OrderDetailSummaryCard()
        {
            return PartialView("Partials/Cards/_OrderDetails");
        }

        [Authorize(Roles = "3")]
        public ActionResult PurchasedItemTable()
        {
            return PartialView("Partials/Tables/_PurchasedItem");
        }

        [Authorize(Roles = "3")]
        public ActionResult OrderSummaryAccordion()
        {
            return PartialView("Partials/Accordions/_OrderSummary");
        }
    }
}