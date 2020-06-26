using OECS.Models;
using OECS.Models.CategoryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CategoryController : Controller
    {
        oecsEntities dbContext = new oecsEntities();
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CategoryList()
        {
            List<CategoryModel> categories = dbContext.ProductDetail
                                                      .Select(s => new CategoryModel
                                                      {
                                                          CategoryID = s.Product.SubCategory.Category.CategoryID,
                                                          Category = s.Product.SubCategory.Category.category1
                                                      }).Distinct().ToList();
            return PartialView("Partials/_CategoryList", categories);
        }

        public ActionResult SubCategoryList(int? categoryID)
        {
            List<SubCategoryModel> subCategoryModels = dbContext.ProductDetail
                                                                .Select(s => new SubCategoryModel
                                                                {
                                                                    SubCategoryID = s.Product.SubCategory.SubCategoryID,
                                                                    SubCategory = s.Product.SubCategory.subCategory1
                                                                }).Distinct().ToList();
            if (categoryID != 0)
            {
                subCategoryModels = dbContext.ProductDetail
                                             .Where(pd => pd.Product.SubCategory.CategoryID == categoryID)
                                             .Select(s => new SubCategoryModel
                                             {
                                                 SubCategoryID = s.Product.SubCategory.SubCategoryID,
                                                 SubCategory = s.Product.SubCategory.subCategory1
                                             }).Distinct().ToList();
            }
            return PartialView("Partials/_SubCategoryList", subCategoryModels);
        }
    }
}