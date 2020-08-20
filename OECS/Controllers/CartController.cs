using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadCartTable()
        {
            return PartialView("Partials/Tables/_Item");
        }

        public ActionResult LoadOrderSummary()
        {
            return PartialView("Partials/Cards/_OrderSummary");
        }
    }
}