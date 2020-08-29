using Microsoft.Owin.Security.Provider;
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
            return PartialView("Partials/Tables/_Item");
        }

        [Authorize(Roles = "3")]
        public ActionResult LoadOrderSummary()
        {
            return PartialView("Partials/Cards/_OrderSummary");
        }

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
                TempData["ProductID"] = viewProductDetailModel.ProductID;
                TempData["ProductDetailID"] = productDetailModel.FirstOrDefault().ProductDetailID;
                foreach (var item in productDetailModel)
                {
                    if(_cartService.CheckDuplicateItem(customerID, (int)item.ProductDetailID) == false)
                    {
                        _cartService.AddItem(customerID, (int)item.ProductDetailID);
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

        private IEnumerable<Claim> GetClaim()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;

            return claims;
        }
    }
}