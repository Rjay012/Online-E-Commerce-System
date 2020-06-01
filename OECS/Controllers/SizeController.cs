using OECS.Models;
using OECS.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class SizeController : Controller
    {
        oecsEntities dbContext = new oecsEntities();
        // GET: Size
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowProductSize(int? productCategoryID)
        {
            if (productCategoryID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ViewProductModel> productSize = dbContext.ProductDetail
                                                          .Select(s => new ViewProductModel
                                                          {
                                                              Size = s.Size,
                                                              Category = (productCategoryID != 0 ? s.Product.Category : null)
                                                          }).Distinct().ToList();
            if (productCategoryID != 0)
            {
                productSize = productSize.Where(s => s.Category.CategoryID == productCategoryID).ToList();
            }
            return PartialView("Partials/_ProductSize", productSize);
        }
    }
}