using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class ProductController : Controller
    {
        oecsEntities dbContext = new oecsEntities();
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show()
        {
            List<Product> product = dbContext.Product.ToList();
            return PartialView("Partials/_ProductList", product);
        }
    }
}