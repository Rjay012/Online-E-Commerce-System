using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using OECS.Models.ProductModels;
using System.Security.Cryptography;

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

        public ActionResult Show(int categoryID, int colorID, string searchString)
        {
            List<ViewProductModel> product = (from p in dbContext.Product
                                              join pi in dbContext.ProductImage on p.ProductID equals pi.ProductID
                                              join pr in dbContext.ProductColor on p.ProductID equals pr.ProductID into k
                                              from l in k.DefaultIfEmpty()
                                              select new ViewProductModel
                                              {
                                                  Product = p, ProductImage = pi, ProductColor = l ?? null, Category = p.Category, Color = l.Color
                                              }).ToList();

            if (!String.IsNullOrEmpty(searchString)) //search string
            {
                product = product.Where(p => p.Category.category1.Contains(searchString) ||
                                             p.Product.productName.Contains(searchString)).ToList();  //you removed color searching
            }

            if (categoryID != 0 && colorID != 0)  //sort combination
            {
                product = product.Where(p => p.Product.Category.CategoryID == categoryID && p.ProductColor.ColorID == colorID && p.ProductImage.isDisplay == true).ToList();
                return PartialView("Partials/_ProductList", product);
            }
            else if (categoryID != 0 && colorID == 0)
            {
                product = product.Where(p => p.Product.Category.CategoryID == categoryID).ToList();
            }
            else if (categoryID == 0 && colorID != 0)
            {
                product = product.Where(c => c.ProductColor.ColorID == colorID && c.ProductImage.isDisplay == true).ToList();
                return PartialView("Partials/_ProductList", product);
            }

            //PagedList<Product> newProduct = new PagedList<Product>(product, page, pageSize);
            return PartialView("Partials/_ProductList", product.Where(i => i.ProductImage.isDisplay == true && i.ProductColor.isDisplay == true).ToList());
        }
    }
}