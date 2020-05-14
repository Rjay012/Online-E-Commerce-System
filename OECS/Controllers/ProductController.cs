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
using EntityFramework.Extensions;

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
                                              join pi in dbContext.ProductImage on pc.ProductColorID equals pi.ProductColorID
                                              select new ViewProductModel
                                              {
                                                  Product = p,
                                                  ProductImage = pi,
                                                  ProductColor = pc
                                              }).ToList();

            if (!String.IsNullOrEmpty(searchString)) //search string
            {
                product = product.Where(p => p.Product.Category.category1.Contains(searchString) ||
                                             p.Product.productName.Contains(searchString)).ToList();  //you removed color searching
            }

            if (categoryID != 0 && colorID != 0)  //sort combination
            {
                product = product.Where(p => p.Product.Category.CategoryID == categoryID && p.ProductColor.Color.ColorID == colorID && p.ProductColor.toDisplay == true).ToList();
                return PartialView("Partials/_ProductList", product);
            }
            else if (categoryID != 0 && colorID == 0)
            {
                product = product.Where(p => p.Product.Category.CategoryID == categoryID).ToList();
            }
            else if (categoryID == 0 && colorID != 0)
            {
                product = product.Where(c => c.ProductColor.Color.ColorID == colorID && c.ProductColor.toDisplay == true).ToList();
                return PartialView("Partials/_ProductList", product);
            }

            //PagedList<Product> newProduct = new PagedList<Product>(product, page, pageSize);
            return PartialView("Partials/_ProductList", product.Where(i => i.ProductImage.isMainDisplay == true).ToList());
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
            ProductImage productImage = new ProductImage();
            if(ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase file = files[0];

                    var fname = Path.GetFileNameWithoutExtension(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    fname = fname + DateTime.Now.ToString("yymmssff") + extension;
                    file.SaveAs(Path.Combine(Server.MapPath("/Images"), fname));

                    /*
                     * this code might be a little bit tricky, if the product has duplicate color e.g 2 greens, set 1 color to (toDisplay) to avoid product duplication
                     * count the number of product with duplicate color, if none set each 1 to true
                     */
                    var noOfDuplicateInColor = dbContext.ProductColor
                                                        .Where(c => c.ProductID == productColorModel.ProductID && c.ColorID == productColorModel.ColorID).Count();

                    bool toDisplay = noOfDuplicateInColor == 0 ? true : false;  //set to Display

                    productImage.isMainDisplay = productColorModel.IsMainDisplay;
                    productImage.path = "Images\\" + fname;
                    productImage.ProductColor = new ProductColor
                    {
                        ProductID = productColorModel.ProductID,
                        ColorID = productColorModel.ColorID,
                        isDisplay = productColorModel.IsDisplay,
                        toDisplay = toDisplay
                    };

                    dbContext.ProductImage.Add(productImage);
                    dbContext.SaveChanges();
                    return Json(productImage, JsonRequestBehavior.AllowGet);
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

            List<ViewProductModel> productImage = dbContext.ProductImage
                                                           .Select(pi => new ViewProductModel
                                                           {
                                                               ProductImage = pi,
                                                               ProductColor = pi.ProductColor,
                                                               Icon = pi.Icon
                                                           }).Where(p => p.ProductColor.ProductID == productID && p.ProductColor.ColorID == colorID && p.ProductImage.IconID == iconID).ToList();
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
            var product = dbContext.ProductImage
                                   .Select(pi => new {
                                       pi.ProductColor.Product.ProductID,
                                       pi.ProductColor.ColorID,
                                       pi.isMainDisplay,
                                       pi.IconID,
                                       pi.ProductColor.Product.productName,
                                       pi.ProductColor.Product.Category.category1,
                                       pi.ProductColor.Product.date,
                                       pi.ProductColor.Product.price
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
        public ActionResult SetDisplay(int? defaultID, int? selectedID)
        {
            if(defaultID == null && selectedID == null)
            {
                return HttpNotFound();
            }

            UpdateDisplay(new ProductColor() { ProductColorID = (int)defaultID, isDisplay = false });  //remove previous display
            UpdateDisplay(new ProductColor() { ProductColorID = (int)selectedID, isDisplay = true });  //set new display

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetMainDisplay(int? productID, int? selectedID)
        {
            if(productID == null && selectedID == null)
            {
                return HttpNotFound();
            }

            UpdateMainDisplay(new ProductColorModel() { ProductID = (int)productID }, true);  //remove previous  main display
            UpdateMainDisplay(new ProductColorModel() { ProductColorID = (int)selectedID }, false);  //set new main display

            return Json("", JsonRequestBehavior.AllowGet);
        }

        private void UpdateMainDisplay(ProductColorModel productColorModel, bool isMainDisplay) //this update uses (ENTITY FRAMEWORK.EXTENDED) for more info see https://www.seguetech.com/performing-bulk-updatesentity-framework-6-1/
        {
            if (ModelState.IsValid)
            {
                if (isMainDisplay == true)
                {
                    dbContext.ProductImage
                             .Where(i => i.ProductColor.ProductID == productColorModel.ProductID && i.isMainDisplay == true)
                             .Update(i => new ProductImage() { isMainDisplay = false });
                }
                else
                {
                    dbContext.ProductImage
                             .Where(i => i.ProductColorID == productColorModel.ProductColorID)
                             .Update(i => new ProductImage() { isMainDisplay = true });
                }
            }
        }

        private void UpdateDisplay(ProductColor productColor)
        {
            if(ModelState.IsValid)
            {
                dbContext.ProductColor.Attach(productColor);
                dbContext.Entry(productColor).Property(i => i.isDisplay).IsModified = true;  //modify and update only 1 property
                dbContext.SaveChanges();
            }
        }
        #endregion ("END SET IMAGE DISPLAY")
    }
}