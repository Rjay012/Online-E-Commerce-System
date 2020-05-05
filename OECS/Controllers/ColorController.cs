using OECS.Models;
using OECS.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OECS.Controllers
{
    public class ColorController : Controller
    {
        oecsEntities dbContext = new oecsEntities();
        // GET: Color
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowProductColor(int productCategoryID)
        {
            List<ViewProductModel> productColor = (from pr in dbContext.ProductColor
                                                   join c in dbContext.Color on pr.ColorID equals c.ColorID
                                                   join p in dbContext.Product on pr.ProductID equals p.ProductID
                                                   join cat in dbContext.Category on p.CategoryID equals cat.CategoryID
                                                   select new ViewProductModel
                                                   {
                                                       Color = c, 
                                                       Category = (productCategoryID != 0 ? cat : null),
                                                   }).Distinct().ToList();

            if(productCategoryID != 0)
            {
                productColor = productColor.Where(c => c.Category.CategoryID == productCategoryID).Distinct().ToList();
            }
            return PartialView("Partials/_ProductColor", productColor);
        }
    }
}