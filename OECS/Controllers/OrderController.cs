using OECS.Models;
using OECS.Models.OrderModels;
using OECS.Repository.OrderRepository;
using OECS.Services.OrderServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController()
        {
            _orderService = new OrderService(new OrderRepository(new oecsEntities()));
        }

        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "3")]
        public ActionResult NewOrderPurchased()
        {
            return View(nameof(NewOrderPurchased));
        }

        [Authorize(Roles = "3")]
        public ActionResult OrderDetailCard()
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);

            ViewBag.TotalAmount = String.Format("{0:C}", _orderService.GetTotalAmount(customerID));
            return PartialView("Partials/Cards/_OrderDetails");
        }

        [Authorize(Roles = "3")]
        public ActionResult PurchasedItemTable()
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            List<OrderModel> orderModels = _orderService.ShowCustomerOrder(customerID);

            if(orderModels == null)
            {
                return HttpNotFound();
            }

            return PartialView("Partials/Tables/_PurchasedItem", orderModels);
        }

        [Authorize(Roles = "3")]
        public ActionResult OrderSummaryAccordion()
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            List<OrderModel> orderModels = _orderService.OrderSummary(customerID);

            if(orderModels == null)
            {
                return HttpNotFound();
            }

            return PartialView("Partials/Accordions/_OrderSummary", orderModels);
        }

        private IEnumerable<Claim> GetClaim()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            return claims;
        }
    }
}