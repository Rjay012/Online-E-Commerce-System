using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using OECS.Models.ProductModels;
using System.Security.Cryptography;
using Microsoft.Ajax.Utilities;

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
                                              join pc in dbContext.ProductColor on p.ProductID equals pc.ProductID
                                              join pi in dbContext.ProductImage on p.ProductID equals pi.ProductID
                                              group new { p, pc, pi } by new
                                              {
                                                  product = p,
                                                  productImage = pi,
                                                  productColor = pc
                                              } into grp
                                              select new ViewProductModel
                                              {
                                                  Product = grp.Key.product,
                                                  Category = grp.Key.product.Category,
                                                  ProductColor = grp.Key.productColor,
                                                  Color = grp.Key.productColor.Color,
                                                  ProductImage = grp.Key.productImage
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