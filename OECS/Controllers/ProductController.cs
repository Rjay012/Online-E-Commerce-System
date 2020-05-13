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
using OECS.Models.CategoryModels;
using System.IO;
using System.Drawing;
using System.Data.Entity;

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
                                              group new { p, pc } by new
                                              {
                                                  product = p,
                                                  productColor = pc
                                              } into grp
                                              select new ViewProductModel
                                              {
                                                  Product = grp.Key.product,
                                                  Category = grp.Key.product.Category,
                                                  ProductColor = grp.Key.productColor,
                                                  Color = grp.Key.productColor.Color
                                              }).ToList();

            if (!String.IsNullOrEmpty(searchString)) //search string
            {
                product = product.Where(p => p.Category.category1.Contains(searchString) ||
                                             p.Product.productName.Contains(searchString)).ToList();  //you removed color searching
            }

            if (categoryID != 0 && colorID != 0)  //sort combination
            {
                product = product.Where(p => p.Product.Category.CategoryID == categoryID && p.ProductColor.Color.ColorID == colorID && p.ProductColor.isMainDisplay == true).ToList();
                return PartialView("Partials/_ProductList", product);
            }
            else if (categoryID != 0 && colorID == 0)
            {
                product = product.Where(p => p.Product.Category.CategoryID == categoryID).ToList();
            }
            else if (categoryID == 0 && colorID != 0)
            {
                product = product.Where(c => c.ProductColor.Color.ColorID == colorID && c.ProductColor.isMainDisplay == true).ToList();
                return PartialView("Partials/_ProductList", product);
            }

            //PagedList<Product> newProduct = new PagedList<Product>(product, page, pageSize);
            return PartialView("Partials/_ProductList", product.Where(i => i.ProductColor.isMainDisplay == true).ToList());
        }

        public ActionResult NewProductModalForm(ProductModel productModel)
        {
            productModel.CategoryList = GetCategoryListItems();
            return PartialView("Partials/Modals/_NewProduct", productModel);
        }

        public ActionResult NewColorModalForm()
        {
            ViewBag.ColorList = GetColorListItems();
            return PartialView("Partials/Modals/_NewProductColor");
        }

        [Authorize(Roles = "1")]
        public ActionResult Create(ProductModel productModel)
        {
            //create logic
            return Json(productModel, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SelectListItem> GetColorListItems()
        {
            List<SelectListItem> ColorListTempStorage = new List<SelectListItem>();
            var color = dbContext.Color.ToList();
            foreach (var item in color)
            {
                ColorListTempStorage.Add(new SelectListItem
                {
                    Value = item.ColorID.ToString(),
                    Text = item.color1
                });
            }

            return ColorListTempStorage;
        }

        [Authorize(Roles = "1")]
        public ActionResult CreateNewProductColor(ProductColorModel productColorModel)
        {
            ProductColor productColor = new ProductColor();
            if(ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    string fname = "", extension = "";

                    HttpPostedFileBase file = files[0];

                    fname = Path.GetFileNameWithoutExtension(file.FileName);
                    extension = Path.GetExtension(file.FileName);
                    fname = fname + DateTime.Now.ToString("yymmssff") + extension;
                    file.SaveAs(Path.Combine(Server.MapPath("/Images"), fname));

                    productColor.ProductID = productColorModel.ProductID;
                    productColor.ColorID = productColorModel.ColorID;
                    productColor.isDisplay = productColorModel.IsDisplay;
                    productColor.isMainDisplay = productColorModel.IsMainDisplay;
                    productColor.path = "Images\\" + fname;

                    dbContext.ProductColor.Add(productColor);
                    dbContext.SaveChanges();
                    return Json(productColorModel, JsonRequestBehavior.AllowGet);
                }
            }
           
            return Json(productColorModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewPhotoGallery(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null)
            {
                return HttpNotFound();
            }

            //List<ViewProductModel> productImage = (from pc in dbContext.ProductColor
            //                                       join i in dbContext.Icon on pc.ProductColorID equals i.ProductColorID into l
            //                                       from n in l.DefaultIfEmpty()
            //                                       select new ViewProductModel
            //                                       {
            //                                           ProductColor = pc,
            //                                           Icon = n ?? null
            //                                       }).Where(p => p.ProductColor.ProductID == productID && p.ProductColor.ColorID == colorID).ToList();
            List<ViewProductModel> productImage = dbContext.ProductColor
                                                           .Select(pc => new ViewProductModel { 
                                                                ProductColor = pc,
                                                                Icon = pc.Icon
                                                           }).Where(p => p.ProductColor.ProductID == productID && p.ProductColor.ColorID == colorID && p.ProductColor.IconID == iconID).ToList();
            ViewBag.ProductID = productID;
            return PartialView("Partials/Modals/_ProductImageGallery", productImage);
        }

        private IEnumerable<SelectListItem> GetCategoryListItems()
        {
            List<SelectListItem> CategoryListTempStorage = new List<SelectListItem>();
            var category = dbContext.Category.ToList();
            foreach (var item in category)
            {
                CategoryListTempStorage.Add(new SelectListItem
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.category1
                });
            }

            return CategoryListTempStorage;
        }

        [Authorize(Roles = "1")]
        public ActionResult ShowProduct(DataTableParam param)
        {
            var product = dbContext.Product
                                   .Join(dbContext.ProductColor, p => p.ProductID, pi => pi.ProductID,
                                   (p, pi) => new
                                   {
                                       p.ProductID,
                                       pi.ColorID,
                                       pi.isMainDisplay,
                                       pi.IconID,
                                       p.productName,
                                       p.Category.category1,
                                       p.date,
                                       p.price
                                   }).Where(pi => pi.isMainDisplay == true).ToList();

            switch (param.iSortCol_0)  //column sorting
            {
                case 1:
                    product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.productName).ToList() : product.OrderByDescending(c => c.productName).ToList();
                    break;
                case 2:
                    product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.category1).ToList() : product.OrderByDescending(c => c.category1).ToList();
                    break;
                case 4:
                    product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.date).ToList() : product.OrderByDescending(c => c.date).ToList();
                    break;
                case 5:
                    product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.price).ToList() : product.OrderByDescending(c => c.price).ToList();
                    break;
            }

            //pagination
            var displayResult = product.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            return Json(new
            {
                aaData = displayResult,
                param.sEcho,
                iTotalRecords = product.Count(),
                iTotalDisplayRecords = product.Count()
            }, JsonRequestBehavior.AllowGet);
        }

        #region ("START SET IMAGE DISPLAY")
        [Authorize(Roles = "1")]
        public ActionResult SetUsDisplay(int? defaultID, int? selectedID)
        {
            UpdateDisplay(new ProductColor() { ProductColorID = (int)defaultID, isDisplay = false });  //remove previous display
            UpdateDisplay(new ProductColor() { ProductColorID = (int)selectedID, isDisplay = true });  //set new display
            return Json("", JsonRequestBehavior.AllowGet);
        }

        private void UpdateDisplay(ProductColor productColor)
        {
            dbContext.ProductColor.Attach(productColor);
            dbContext.Entry(productColor).Property(i => i.isDisplay).IsModified = true;  //modify and update only 1 property
            dbContext.SaveChanges();
        }
        #endregion ("END SET IMAGE DISPLAY")
    }
}