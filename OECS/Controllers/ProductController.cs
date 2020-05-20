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
using System.Net;
using Icon = OECS.Models.Icon;

namespace OECS.Controllers
{
    public class ProductController : Controller
    {
        oecsEntities dbContext = new oecsEntities();
        // GET: Product
        [Authorize(Roles = "1,2,3")]
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

        #region ("START MODAL FORMS")
        public ActionResult NewProductModalForm(ProductModel productModel)
        {
            productModel.CategoryList = GetCategoryListItems();
            return PartialView("Partials/Modals/_NewProduct", productModel);
        }

        public ActionResult NewColorModalForm(ProductColorModel productColorModel)
        {
            productColorModel.ColorList = GetColorListItems();
            return PartialView("Partials/Modals/_NewProductColor", productColorModel);
        }

        public ActionResult EditColorModalForm(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null && iconID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ProductImage> productImage = dbContext.ProductImage
                                                       .Where(i => i.ProductColor.ProductID == productID && i.ProductColor.ColorID == colorID && i.IconID == iconID).ToList();

            ProductColorModel productColorModel = new ProductColorModel
            {
                ProductImage = productImage.ToList(),
                ColorList = GetColorListItems(),
                ProductID = (int)productID,
                ProductColorID = (int)productImage.Select(c => new { c.ProductColorID }).FirstOrDefault().ProductColorID
            };
            return PartialView("Partials/Modals/_EditProductColor", productColorModel);
        }
        #endregion ("END MODAL FORMS")

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

        private ActionResult CreateNewIcon([Bind(Include = "IconPath")] ProductColorModel productColorModel)
        {
            if(productColorModel == null)
            {
                return HttpNotFound();
            }

            if(ModelState.IsValid)
            {
                Models.Icon icon = new Models.Icon();
                icon.icon1 = productColorModel.IconPath;

                dbContext.Icon.Add(icon);
                dbContext.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private string UploadFile(HttpPostedFileBase file, string directory)  //upload file to directory
        {
            var fname = Path.GetFileNameWithoutExtension(file.FileName);
            var extension = Path.GetExtension(file.FileName);
            fname = fname + DateTime.Now.ToString("yymmssff") + extension;
            file.SaveAs(Path.Combine(Server.MapPath(directory), fname));

            return fname;
        }

        [Authorize(Roles = "1")]
        public ActionResult CreateNewProductColor(ProductColorModel productColorModel)
        {
            if (productColorModel.IconFile == null || productColorModel.Files == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductImage productImage = new ProductImage();
            if (ModelState.IsValid)
            {
                int position = 1;
                string fname = productColorModel.Path;

                //upload new icon
                productColorModel.IconPath = "Images\\AddImageIcon\\" + UploadFile(productColorModel.IconFile, "/Images/AddImageIcon");
                CreateNewIcon(new ProductColorModel() { IconPath = productColorModel.IconPath });

                foreach (HttpPostedFileBase file in productColorModel.Files)
                {
                    fname = UploadFile(file, "/Images");
                    productColorModel.IsDisplay = (position == productColorModel.IsDisplayPosition ? true : false);  //find isDisplay position

                    /*
                     * this code might be a little bit tricky, if the product has duplicate color e.g 2 greens, set 1 color to (toDisplay) to avoid product duplication
                     * count the number of product with duplicate color, if none set each 1 to true
                     */
                    int noOfDuplicateInColor = dbContext.ProductColor
                                                        .Where(c => c.ProductID == productColorModel.ProductID && c.ColorID == productColorModel.ColorID).Count();
                    productColorModel.ToDisplay = (noOfDuplicateInColor == 0 && productImage.path != "Images\\AddImageIcon\\add-image-icon.png" ? true : false); //avoid display default image if product color has been duplicated

                    bool findMainDisplay = dbContext.ProductImage
                                                    .Where(c => c.ProductColor.ProductID == productColorModel.ProductID && c.isMainDisplay == true).Any();

                    productImage.isMainDisplay = (findMainDisplay == true ? false : true);  //for each product color, only 1 color can be set to main display
                    productImage.path = "Images\\" + fname;
                    productImage.IconID = dbContext.Icon.Max(i => i.IconID);  //get newly added icon
                    productImage.ProductColor = new ProductColor
                    {
                        ProductID = productColorModel.ProductID,
                        ColorID = productColorModel.ColorID,
                        isDisplay = productColorModel.IsDisplay,
                        toDisplay = productColorModel.ToDisplay
                    };

                    dbContext.ProductImage.Add(productImage);
                    dbContext.SaveChanges();
                    position++;
                }
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "1")]
        public ActionResult EditProductImage(ProductColorModel productColorModel)  //via jquer ajax request
        {
            ProductImage productImage = new ProductImage();

            if(ModelState.IsValid)
            {
                if (Request.Files.Count > 0)  //update the images to database and directory
                {
                    HttpFileCollectionBase files = Request.Files;
                    HttpPostedFileBase file = files[0];

                    //remove the file first
                    productImage = dbContext.ProductImage.Find(productColorModel.ImageID);
                    string deletePath = Server.MapPath("~\\" + productImage.path);
                    if (System.IO.File.Exists(deletePath))
                    {
                        System.IO.File.Delete(deletePath);
                    }

                    //upload the new image
                    productImage.ImageID = productColorModel.ImageID;
                    productImage.path = "Images\\" + UploadFile(file, "/Images");

                    dbContext.ProductImage.Attach(productImage);
                    dbContext.Entry(productImage).Property(i => i.path).IsModified = true;
                    dbContext.SaveChanges();
                }
            }
            
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "1")]
        public ActionResult EditProductColor([Bind(Include = "ProductID, ProductColorID, ColorID")] ProductColorModel productColorModel)  //via unobtrussive ajax request
        {
            ProductColor productColor = new ProductColor();
            if(ModelState.IsValid)
            {
                bool hasColorDuplicate = dbContext.ProductColor
                                                  .Where(c => c.ProductID == productColorModel.ProductID && c.ColorID == productColorModel.ColorID).Any();
                productColorModel.ToDisplay = hasColorDuplicate == true ? false : true;

                for(int i=1; i<=5; i++)  
                {
                    dbContext.ProductColor
                             .Where(c => c.ProductID == productColorModel.ProductID && c.ProductColorID == productColorModel.ProductColorID)
                             .Update(c => new ProductColor { ColorID = productColorModel.ColorID, toDisplay = productColorModel.ToDisplay });
                    productColorModel.ProductColorID += 1;
                    productColorModel.ToDisplay = false;  //only first data to update will be true the rest is false
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult ViewPhotoGallery(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                                   .Select(pi => new
                                   {
                                       pi.ProductColor.Product.ProductID,
                                       pi.ProductColor.ColorID,
                                       pi.isMainDisplay,
                                       pi.IconID,
                                       pi.ProductColor.Product.productName,
                                       pi.ProductColor.Product.Category.category1,
                                       pi.ProductColor.Product.date,
                                       pi.ProductColor.Product.price
                                   }).Where(pi => pi.isMainDisplay == true).ToList();

            if (!String.IsNullOrEmpty(param.sSearch))
            {
                product = product.Where(p => p.productName.ToLower().Contains(param.sSearch) ||
                                             p.category1.ToLower().Contains(param.sSearch)).ToList();
            }

            switch (param.iSortCol_0)  //column sorting
            {
                case 0:
                    product = product.OrderByDescending(c => c.date).ToList();
                    break;
                case 3:
                    product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.productName).ToList() : product.OrderByDescending(c => c.productName).ToList();
                    break;
                case 4:
                    product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.category1).ToList() : product.OrderByDescending(c => c.category1).ToList();
                    break;
                case 6:
                    product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.date).ToList() : product.OrderByDescending(c => c.date).ToList();
                    break;
                case 7:
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
            if (defaultID == null && selectedID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UpdateDisplay(new ProductColor() { ProductColorID = (int)defaultID, isDisplay = false });  //remove previous display
            UpdateDisplay(new ProductColor() { ProductColorID = (int)selectedID, isDisplay = true });  //set new display

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetMainDisplay(int? productID, int? selectedID)
        {
            if (productID == null && selectedID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
                             .Where(i => i.ProductColor.ProductID == productColorModel.ProductID && i.isMainDisplay == true)  //remove old main display
                             .Update(i => new ProductImage() { isMainDisplay = false });
                }
                else
                {
                    dbContext.ProductImage
                             .Where(i => i.ProductColorID == productColorModel.ProductColorID)  //set new main display
                             .Update(i => new ProductImage() { isMainDisplay = true });
                }
            }
        }

        private void UpdateDisplay(ProductColor productColor)
        {
            if (ModelState.IsValid)
            {
                dbContext.ProductColor.Attach(productColor);
                dbContext.Entry(productColor).Property(i => i.isDisplay).IsModified = true;  //modify and update only 1 property
                dbContext.SaveChanges();
            }
        }
        #endregion ("END SET IMAGE DISPLAY")
    }
}