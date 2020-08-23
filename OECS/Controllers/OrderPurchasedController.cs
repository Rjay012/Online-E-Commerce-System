using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class OrderPurchasedController : Controller
    {
        // GET: OrderPurchased
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PurchasedGreetingCard()
        {
            return PartialView("Partials/Cards/_PurchasedGreetings");
        }

        public ActionResult OrderDetailSummaryCard()
        {
            return PartialView("Partials/Cards/_OrderDetails");
        }

        public ActionResult PurchasedItemTable()
        {
            return PartialView("Partials/Tables/_PurchasedItem");
        }

        public ActionResult OrderSummaryAccordion()
        {
            return PartialView("Partials/Accordions/_OrderSummary");
        }
    }
}