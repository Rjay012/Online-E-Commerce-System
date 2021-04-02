using OECS.Models;
using OECS.Models.OrderModels;
using OECS.Models.OrderModels.PaymentModels;
using OECS.Repository.OrderRepository.PaymentRepository;
using OECS.Services.OrderServices.PaymentServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;

        public PaymentController()
        {
            _paymentService = new PaymentService(new PaymentRepository(new oecsEntities()));
        }

        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConfirmPayment(PaymentModel paymentModel)
        {
            if(paymentModel == null)
            {
                return HttpNotFound();
            }

            List<OrderModel> orderModels = TempData["OrderList"] as List<OrderModel>;

            if(orderModels == null)
            {
                return HttpNotFound();
            }

            paymentModel.CustomerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            _paymentService.CreatePayment(paymentModel, orderModels);

            return null;
        }

        private IEnumerable<Claim> GetClaim()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            return claims;
        }
    }
}