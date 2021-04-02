using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using OECS.Models;
using OECS.Models.CartModels;
using OECS.Models.CustomerModels;
using OECS.Models.OrderModels;
using OECS.Models.OrderModels.PaymentModels;
using OECS.Repository.CartRepository;
using OECS.Repository.CheckoutRepository;
using OECS.Repository.CustomerRepository;
using OECS.Repository.OrderRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using OECS.Services.CartServices;
using OECS.Services.CheckoutServices;
using OECS.Services.CustomerServices;
using OECS.Services.OrderServices;

namespace OECS.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        private readonly ICartService _cartService;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        public CheckOutController()
        {
            _checkoutService = new CheckoutService(new CheckoutRepository(new oecsEntities()));
            _cartService = new CartService(new CartRepository(new oecsEntities()), new ProductDetailRepository(new oecsEntities()));
            _customerService = new CustomerService(new CustomerRepository(new oecsEntities()));
            _orderService = new OrderService(new OrderRepository(new oecsEntities()));
        }

        [Authorize(Roles = "3")]
        // GET: CheckOut
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "3")]
        public ActionResult LoadPackage()
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            List<ViewCartItem> viewCartItems = _cartService.LoadCart(customerID)
                                                           .Where(c => c.Status == "checkout")
                                                           .OrderByDescending(c => c.OrderNo)
                                                           .ToList();

            return PartialView("Partials/Tables/_Package", viewCartItems);
        }

        [Authorize(Roles = "3")]
        public ActionResult ShippingAndOrderSummary()
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            Customer customer = _customerService.GetCustomer(customerID);

            if(customer == null)
            {
                return HttpNotFound();
            }

            CustomerModel customerModel = new CustomerModel()
            {
                Firstname = customer.fname,
                Middlename = customer.mname,
                Lastname = customer.lname,
                Email = customer.email,
                Phone = customer.phone,
                Address = customer.address
            };

            return PartialView("Partials/Cards/_ShippingAndOrderSummary", customerModel);
        }

        [Authorize(Roles = "3")]
        public ActionResult CheckOrderNumberAvailability(string orderNumber)
        {
            return Json(new { isAvailable = _checkoutService.CheckAvailableOrderNumber(orderNumber) }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "3")]
        public ActionResult GetCheckoutItem()
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            List<ViewCartItem> viewCartItems = _cartService.LoadCart(customerID)
                                                           .Where(c => c.Status == "checkout")
                                                           .ToList();
            return Json(new { CheckoutItem = viewCartItems }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "3")]
        public ActionResult Checkout(List<OrderModel> orders)
        {
            if(orders == null)
            {
                return HttpNotFound();
            }

            TempData["OrderList"] = orders;
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            _orderService.CreateOrder(customerID, orders);
            _cartService.GetCheckoutItem(customerID);  //get checkout item and set its status to unavailable

            return null;
        }

        [Authorize(Roles = "3")]
        public ActionResult PaymentMethodConfirmationModal(PaymentModel paymentModel)
        {
            paymentModel.PaymentDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            return PartialView("Partials/Modals/_PaymentMethodConfirmation", paymentModel);
        } 

        private IEnumerable<Claim> GetClaim()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            return claims;
        }
    }
}