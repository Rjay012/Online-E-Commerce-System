using OECS.Models;
using OECS.Models.BrandModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class BrandController : Controller
    {
        private oecsEntities dbContext = new oecsEntities();
        // GET: Brand
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BrandList(int? categoryID, int? subCategoryID)
        {
            List<BrandModel> brandModel = dbContext.ProductDetail
                                                   .Select(s => new BrandModel
                                                   {
                                                       BrandID = (int)s.Product.BrandID,
                                                       BrandName = s.Product.Brand.BrandName,
                                                       CategoryID = categoryID == 0 ? null : s.Product.SubCategory.CategoryID,
                                                       SubCategoryID = subCategoryID == 0 ? null : s.Product.SubCategoryID
                                                   }).Distinct().ToList();

            if (categoryID != 0) { brandModel = brandModel.Where(pd => pd.CategoryID == categoryID).ToList(); }
            if (subCategoryID != 0) { brandModel = brandModel.Where(pd => pd.SubCategoryID == subCategoryID).ToList(); }
            return PartialView("Partials/_BrandList", brandModel);
        }
    }
}