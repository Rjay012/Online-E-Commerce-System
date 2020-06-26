using OECS.Models;
using OECS.Models.ProductDetailModels.SizeModels;
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

        public ActionResult ShowProductSize(int? categoryID, int? subCategoryID)
        {
            List<SizeModel> sizeModel = dbContext.ProductDetail
                                                 .Select(s => new SizeModel
                                                 {
                                                     SideID = (int)s.SizeID,
                                                     Size = s.Size.size1,
                                                     CategoryID = categoryID == 0 ? null : s.Product.SubCategory.CategoryID,
                                                     SubCategoryID = subCategoryID == 0 ? null : s.Product.SubCategoryID
                                                 }).Distinct().ToList();

            if (categoryID != 0) { sizeModel = sizeModel.Where(pd => pd.CategoryID == categoryID).ToList(); }
            if (subCategoryID != 0) { sizeModel = sizeModel.Where(pd => pd.SubCategoryID == subCategoryID).ToList(); }

            return PartialView("Partials/_ProductSize", sizeModel);
        }
    }
}