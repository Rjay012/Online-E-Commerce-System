using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CartController : Controller
    {
        [Authorize(Roles = "3")]
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "3")]
        public ActionResult LoadCartTable()
        {
            return PartialView("Partials/Tables/_Item");
        }

        [Authorize(Roles = "3")]
        public ActionResult LoadOrderSummary()
        {
            return PartialView("Partials/Cards/_OrderSummary");
        }
    }
}