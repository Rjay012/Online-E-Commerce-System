using OECS.Models;
using OECS.Models.CartModels;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Repository.CartRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using OECS.Services.CartServices;
using OECS.Services.ProductServices.ProductDetailServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductDetailService _productDetailService;
        public CartController()
        {
            _cartService = new CartService(new CartRepository(new oecsEntities()));
            _productDetailService = new ProductDetailService(new ProductDetailRepository(new oecsEntities()));
        }

        [Authorize(Roles = "3")]
        // GET: Cart
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "3")]
        public ActionResult LoadCartTable()
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            List<ViewCartItem> viewCartItems = _cartService.LoadCart(customerID)
                                                           .OrderByDescending(c => c.OrderNo)
                                                           .ToList();

            return PartialView("Partials/Tables/_Item", viewCartItems);
        }

        [Authorize(Roles = "3")]
        public ActionResult LoadOrderSummary()
        {
            return PartialView("Partials/Cards/_OrderSummary");
        }

        #region "ViewFullDetails Page"
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "3")]
        public ActionResult AddItem([Bind(Include = "ProductID, ColorID, SizeID, Quantity")]ViewProductDetailModel viewProductDetailModel)
        {
            if (viewProductDetailModel == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                List<ProductDetailModel> productDetailModel = _productDetailService.ProductDetailList(viewProductDetailModel)
                                                                                   .Take(viewProductDetailModel.Quantity)
                                                                                   .ToList();
                int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
                int orderNo = (int)_cartService.CustomerCartLastOrderNo(customerID) + 1;

                TempData["ProductID"] = viewProductDetailModel.ProductID;
                TempData["ProductDetailID"] = productDetailModel.FirstOrDefault().ProductDetailID;

                foreach (var item in productDetailModel)
                {
                    if (_cartService.CheckDuplicateItem(customerID, (int)item.ProductDetailID) == false)
                    {
                        _cartService.AddItem(customerID, (int)item.ProductDetailID, orderNo);
                    }
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "3")]
        public ActionResult ViewAddedItem(int quantity)
        {
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            CartModel cartModel = _cartService.ViewAddedItem((int)TempData["ProductID"], customerID, quantity);

            ViewBag.ProductID = TempData["ProductID"];
            ViewBag.ProductDetailID = TempData["ProductDetailID"];
            return PartialView("Partials/Modals/_AddedItem", cartModel);
        }
        #endregion

        [Authorize(Roles = "1,3")]
        public ActionResult Delete(List<int> orderNo)
        {
            if(orderNo == null)
            {
                return HttpNotFound();
            }

            //delete process
            int customerID = Convert.ToInt32(GetClaim().ElementAt(0).Value);
            foreach (var item in orderNo)
            {
                _cartService.DeleteItem(customerID, item);
            }
            return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<Claim> GetClaim()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            return claims;
        }
    }
}