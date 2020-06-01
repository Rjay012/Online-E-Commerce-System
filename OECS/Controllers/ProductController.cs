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
using System.Web.UI.WebControls;
using Image = OECS.Models.Image;

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

        public ActionResult Show(int categoryID, int colorID, int sizeID, string searchString)
        {
            List<ViewProductModel> product = dbContext.ProductImage
                                                      .Select(p => new ViewProductModel
                                                      {
                                                          Product = p.ProductDetail.Product,
                                                          ProductImage = p,
                                                          ProductDetail = p.ProductDetail
                                                      }).ToList();
            List<ViewProductModel> displayColor = new List<ViewProductModel>();
            List<ViewProductModel> displaySize = new List<ViewProductModel>();
            if (colorID != 0)
            {
                displayColor = dbContext.DisplayColor
                                        .Select(d => new ViewProductModel
                                        {
                                            Product = d.ProductImage.ProductDetail.Product,
                                            ProductDetail = d.ProductImage.ProductDetail,
                                            ProductImage = d.ProductImage,
                                            DisplayColor = d
                                        }).ToList();
            }

            if (sizeID != 0)
            {
                displaySize = dbContext.DisplaySize
                                        .Select(d => new ViewProductModel
                                        {
                                            Product = d.ProductImage.ProductDetail.Product,
                                            ProductDetail = d.ProductImage.ProductDetail,
                                            ProductImage = d.ProductImage,
                                            DisplaySize = d
                                        }).ToList();
            }


            if (!String.IsNullOrEmpty(searchString)) //search string
            {
                product = product.Where(p => p.Product.Category.category1.Contains(searchString) ||
                                             p.Product.productName.Contains(searchString)).ToList();  //you removed color searching
            }

            //sort combination color
            if (categoryID != 0 && colorID != 0 && sizeID == 0)
            {
                displayColor = displayColor.Where(p => p.Product.Category.CategoryID == categoryID && p.ProductDetail.Color.ColorID == colorID && p.DisplayColor.isDisplay == true).ToList();
                return PartialView("Partials/_ProductList", displayColor);
            }
            else if (categoryID != 0 && sizeID != 0 && colorID == 0)
            {
                displaySize = displaySize.Where(p => p.Product.Category.CategoryID == categoryID && p.ProductDetail.SizeID == sizeID && p.DisplaySize.isDisplay == true).ToList();
                return PartialView("Partials/_ProductList", displaySize);
            }
            else if (categoryID != 0 && colorID == 0 && sizeID == 0)
            {
                product = product.Where(p => p.Product.Category.CategoryID == categoryID).ToList();
            }
            else if (categoryID == 0 && colorID != 0 && sizeID == 0)
            {
                displayColor = displayColor.Where(d => d.ProductDetail.ColorID == colorID && d.DisplayColor.isDisplay == true).ToList();
                return PartialView("Partials/_ProductList", displayColor);
            }
            else if (categoryID == 0 && colorID == 0 && sizeID != 0)
            {
                displaySize = displaySize.Where(d => d.ProductDetail.SizeID == sizeID && d.DisplaySize.isDisplay == true).ToList();
                return PartialView("Partials/_ProductList", displaySize);
            }

            //PagedList<Product> newProduct = new PagedList<Product>(product, page, pageSize);
            return PartialView("Partials/_ProductList", product.Where(i => i.ProductImage.isMainDisplay == true).ToList());
        }

        #region ("START MODAL FORMS")
        public ActionResult NewProductModalForm(ProductModel productModel)
        {
            return PartialView("Partials/Modals/_NewProduct", productModel);
        }

        public ActionResult NewColorModalForm(ProductColorModel productColorModel)
        {
            return PartialView("Partials/Modals/_NewProductColor", productColorModel);
        }

        /*public ActionResult EditColorModalForm(int? productID, int? colorID, int? iconID)
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
                ProductID = (int)productID,
                ProductColorID = (int)productImage.Select(c => new { c.ProductColorID }).FirstOrDefault().ProductColorID
            };
            return PartialView("Partials/Modals/_EditProductColor", productColorModel);
        }*/

        public ActionResult AddSizeModalForm(ProductSizeModel productSizeModel)
        {
            return PartialView("Partials/Modals/_NewProductSize", productSizeModel);
        }
        #endregion ("END MODAL FORMS")

        #region ("START CREATE PRODUCT AND ITS DETAILS")
        [Authorize(Roles = "1")]
        public ActionResult Create(ProductModel productModel)
        {
            //create logic
            return Json(productModel, JsonRequestBehavior.AllowGet);
        }

        private ActionResult CreateNewIcon([Bind(Include = "IconPath")] ProductColorModel productColorModel)
        {
            if (productColorModel == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                Models.Icon icon = new Models.Icon();
                icon.icon1 = productColorModel.IconPath;

                dbContext.Icon.Add(icon);
                dbContext.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private List<int> CreateNewProductDetail([Bind(Include = "ProductID, ColorID, SizeID")] ProductColorModel productColorModel)
        {
            ProductDetail productDetail = new ProductDetail();
            List<int> lstProductDetailID = new List<int>();
            if (ModelState.IsValid)
            {
                int[] arrSize = productColorModel.SizeID.Where(s => s != 0).ToArray();
                foreach (int s in arrSize)
                {
                    productDetail.ProductID = productColorModel.ProductID;
                    productDetail.ColorID = productColorModel.ColorID;
                    productDetail.SizeID = s;
                    dbContext.ProductDetail.Add(productDetail);
                    dbContext.SaveChanges();

                    lstProductDetailID.Add(dbContext.ProductDetail.Max(d => d.ProductDetailID));
                }
            }
            return lstProductDetailID;
        }

        private Dictionary<int, bool> UploadImage([Bind(Include = "Files, IsDisplayPosition")] ProductColorModel productColorModel)
        {
            Image image = new Image();
            Dictionary<int, bool> lstImageID = new Dictionary<int, bool>();
            int position = 1;
            if (ModelState.IsValid)
            {
                foreach (HttpPostedFileBase file in productColorModel.Files)
                {
                    string fname = UploadFile(file, "/Images");
                    image.path = "Images\\" + fname;
                    image.IconID = dbContext.Icon.Max(i => i.IconID);
                    dbContext.Image.Add(image);
                    dbContext.SaveChanges();

                    if (position == productColorModel.IsDisplayPosition)
                    {
                        lstImageID.Add(dbContext.Image.Max(i => i.ImageID), true);  //set us display
                    }
                    else
                    {
                        lstImageID.Add(dbContext.Image.Max(i => i.ImageID), false);
                    }
                    position++;
                }
            }
            return lstImageID;
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

            if (ModelState.IsValid)
            {
                ProductImage productImage = new ProductImage();
                string fname = productColorModel.Path;

                //upload new icon
                productColorModel.IconPath = "Images\\AddImageIcon\\" + UploadFile(productColorModel.IconFile, "/Images/AddImageIcon");
                CreateNewIcon(new ProductColorModel() { IconPath = productColorModel.IconPath });
                List<int> lstProductDetailID = CreateNewProductDetail(new ProductColorModel { ProductID = productColorModel.ProductID, ColorID = productColorModel.ColorID, SizeID = productColorModel.SizeID });
                Dictionary<int, bool> lstImageID = UploadImage(new ProductColorModel { Files = productColorModel.Files, IsDisplayPosition = productColorModel.IsDisplayPosition });

                foreach (int productDetailID in lstProductDetailID)
                {
                    foreach (int imageID in lstImageID.Keys)
                    {
                        bool findMainDisplay = dbContext.ProductDetail
                                                        .Where(d => d.ProductID == productColorModel.ProductID).Any();
                        productImage.ProductDetailID = productDetailID;
                        productImage.ImageID = imageID;
                        productImage.isMainDisplay = (findMainDisplay == true ? false : true);  //for each product color, only 1 color can be set to main display

                        dbContext.ProductImage.Add(productImage);
                        dbContext.SaveChanges();
                    }
                }

                //add display color and size
                CreateDisplayDetail(new ProductColorModel { ProductID = productColorModel.ProductID }, lstImageID.Where(i => i.Value == true).FirstOrDefault().Key);
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult CreateDisplayColor([Bind(Include = "ProductImageID, ProductID, ColorID")] ProductColorModel productColorModel)
        {
            if(ModelState.IsValid)
            {
                DisplayColor displayColor;
                bool hasColor = dbContext.ProductImage
                                         .Where(pi => pi.ProductDetail.ProductID == productColorModel.ProductID && pi.ProductDetail.ColorID == productColorModel.ColorID)
                                         .Join(dbContext.DisplayColor, pi => pi.ProductImageID, dc => dc.ProductImageID, (pi, dc) => new
                                         {
                                             dc.ProductImageID
                                         }).Any();
                
                if(hasColor == true)  
                {
                    displayColor = new DisplayColor();
                    displayColor.DisplayColorID = dbContext.DisplayColor
                                                           .Where(dc => dc.ProductImage.ProductDetail.ProductID == productColorModel.ProductID && dc.ProductImage.ProductDetail.ColorID == productColorModel.ColorID && dc.isDisplay == true)
                                                           .Join(dbContext.ProductImage, dc => dc.ProductImageID, pi => pi.ProductImageID, (dc, pi) => new
                                                           {
                                                               dc.DisplayColorID
                                                           }).FirstOrDefault().DisplayColorID;
                    displayColor.isDisplay = false;
                    dbContext.DisplayColor.Attach(displayColor);
                    dbContext.Entry(displayColor).Property(dc => dc.isDisplay).IsModified = true;
                    dbContext.SaveChanges();
                }

                displayColor = new DisplayColor();
                displayColor.ProductImageID = productColorModel.ProductImageID;
                displayColor.isDisplay = true;
                dbContext.DisplayColor.Add(displayColor);
                dbContext.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult CreateDisplayDetail([Bind(Include = "ProductID")] ProductColorModel productColorModel, int imgID)
        {
            List<ProductColorModel> productImage = dbContext.ProductImage
                                         .Where(pi => pi.ImageID == imgID)
                                         .Select(pi => new ProductColorModel { ProductImageID =  pi.ProductImageID, SID = (int)pi.ProductDetail.SizeID, ColorID = (int)pi.ProductDetail.ColorID }).ToList();
            CreateDisplayColor(new ProductColorModel() { ProductImageID = productImage.FirstOrDefault().ProductImageID, ProductID = productColorModel.ProductID, ColorID = productImage.FirstOrDefault().ColorID });
            CreateDisplaySize(productImage, productColorModel.ProductID);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult CreateDisplaySize(List<ProductColorModel> productColorList, int ProductID)
        {
            DisplaySize displaySize;
            foreach (var i in productColorList)
            {
                bool hasSize = dbContext.ProductImage
                                        .Where(pi => pi.ProductDetail.ProductID == ProductID && pi.ProductDetail.SizeID == i.SID)
                                        .Join(dbContext.DisplaySize, pi => pi.ProductImageID, ds => ds.ProductImageID, (pi, ds) => new { ds.ProductImageID })
                                        .Any();

                if(hasSize == true)
                {
                    displaySize = new DisplaySize();
                    displaySize.DisplaySizeID = dbContext.DisplaySize
                                                         .Where(ds => ds.ProductImage.ProductDetail.ProductID == ProductID && ds.ProductImage.ProductDetail.SizeID == i.SID && ds.isDisplay == true)
                                                         .Select(ds => new { ds.DisplaySizeID }).FirstOrDefault().DisplaySizeID;
                    displaySize.isDisplay = false;
                    dbContext.DisplaySize.Attach(displaySize);
                    dbContext.Entry(displaySize).Property(ds => ds.isDisplay).IsModified = true;
                    dbContext.SaveChanges();
                }
                displaySize = new DisplaySize();
                displaySize.ProductImageID = i.ProductImageID;
                displaySize.isDisplay = true;
                dbContext.DisplaySize.Add(displaySize);
                dbContext.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        #endregion ("END CREATE PRODUCT AND ITS DETAILS")

        /*[Authorize(Roles = "1")]
        public ActionResult EditProductImage(ProductColorModel productColorModel)  //via jquer ajax request
        {
            ProductImage productImage = new ProductImage();

            if (ModelState.IsValid)
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
        }*/

        /*[Authorize(Roles = "1")]
        public ActionResult EditProductColor([Bind(Include = "ProductID, ProductColorID, ColorID")] ProductColorModel productColorModel)  //via unobtrussive ajax request
        {
            ProductColor productColor = new ProductColor();
            if (ModelState.IsValid)
            {
                bool hasColorDuplicate = dbContext.ProductColor
                                                  .Where(c => c.ProductID == productColorModel.ProductID && c.ColorID == productColorModel.ColorID).Any();
                productColorModel.ToDisplay = hasColorDuplicate == true ? false : true;

                productColor.ProductColorID = productColorModel.ProductColorID;
                productColor.ColorID = productColorModel.ColorID;
                productColor.toDisplay = productColorModel.ToDisplay;

                dbContext.ProductColor.Attach(productColor);
                dbContext.Entry(productColor).Property(c => c.ColorID).IsModified = true;
                dbContext.Entry(productColor).Property(c => c.toDisplay).IsModified = true;
                dbContext.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }*/

        #region ("START VIEWING AND MANAGING GALLERY")
        public ActionResult ViewPhotoGallery(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ViewProductModel> productImage = dbContext.ProductImage
                                               .Select(i => new ViewProductModel
                                               {
                                                   ProductImage = i
                                               }).Where(d => d.ProductImage.ProductDetail.ProductID == productID && d.ProductImage.ProductDetail.ColorID == colorID && d.ProductImage.Image.IconID == iconID).ToList();
            ViewBag.ProductID = productID;
            return PartialView("Partials/Modals/_ProductImageGallery", productImage);
        }

        [Authorize(Roles = "1")]
        public ActionResult SetImageDisplay()
        {

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        #endregion ("END VIEWING AND MANAGING GALLERY")

        [Authorize(Roles = "1")]
        public ActionResult ShowProduct(DataTableParam param)
        {
            var product = dbContext.ProductImage
                                   .Select(pi => new
                                   {
                                       pi.ProductDetail.Product.ProductID,
                                       pi.ProductDetail.ColorID,
                                       pi.ProductDetail.Product.display,
                                       pi.isMainDisplay,
                                       pi.Image.IconID,
                                       pi.ProductDetail.Product.productName,
                                       pi.ProductDetail.Product.Category.category1,
                                       pi.ProductDetail.Product.date,
                                       pi.ProductDetail.Product.price
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

        /*#region ("START SET IMAGE DISPLAY")
        [Authorize(Roles = "1")]
        public ActionResult SetDisplay(int? defaultID, int? selectedID)
        {
            if (defaultID == null && selectedID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UpdateDisplay(new ProductImage() { ImageID = (int)defaultID, isDisplay = false });  //remove previous display
            UpdateDisplay(new ProductImage() { ImageID = (int)selectedID, isDisplay = true });  //set new display

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetMainDisplay(int? productID, int? selectedID)
        {
            if (productID == null && selectedID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UpdateMainDisplay(new ProductColorModel() { ProductID = (int)productID }, true);  //remove previous  main display
            UpdateMainDisplay(new ProductColorModel() { ImageID = (int)selectedID }, false);  //set new main display

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
                             .Where(i => i.ImageID == productColorModel.ImageID)  //set new main display
                             .Update(i => new ProductImage() { isMainDisplay = true });
                }
            }
        }

        private void UpdateDisplay(ProductImage productImage)
        {
            if (ModelState.IsValid)
            {
                dbContext.ProductImage.Attach(productImage);
                dbContext.Entry(productImage).Property(i => i.isDisplay).IsModified = true;  //modify and update only 1 property
                dbContext.SaveChanges();
            }
        }
        #endregion ("END SET IMAGE DISPLAY")*/
    }
}