using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels.ColorModels;
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
            ViewBag.ModuleTitle = "Size";
            return View();
        }

        public ActionResult ShowProductColor(int? categoryID, int? subCategoryID)
        {
            List<ColorModel> colorModel = dbContext.ProductDetail
                                                   .Select(s => new ColorModel
                                                   {
                                                       ColorID = (int)s.ColorID,
                                                       Color = s.Color.color1,
                                                       CategoryID = categoryID == 0 ? null : s.Product.SubCategory.CategoryID,
                                                       SubCategoryID = subCategoryID == 0 ? null : s.Product.SubCategoryID
                                                   }).Distinct().ToList();

            if (categoryID != 0) { colorModel = colorModel.Where(pd => pd.CategoryID == categoryID).ToList(); }
            if (subCategoryID != 0) { colorModel = colorModel.Where(pd => pd.SubCategoryID == subCategoryID).ToList(); }

            return PartialView("Partials/_ProductColor", colorModel);
        }
    }
}