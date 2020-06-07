using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using OECS.Models.ProductModels;
using System.IO;
using System.Data.Entity;
using System.Net;
using Icon = OECS.Models.Icon;
using System.Web.UI.WebControls;
using Image = OECS.Models.Image;
using System.Linq.Dynamic;
using Size = OECS.Models.Size;

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

        public ActionResult EditColorModalForm(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null && iconID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ProductImage> productImage = dbContext.ProductImage
                                                       .Where(i => i.ProductDetail.ProductID == productID && i.ProductDetail.ColorID == colorID && i.Image.IconID == iconID).ToList();

            ProductColorModel productColorModel = new ProductColorModel
            {
                ProductImage = productImage.ToList(),
                ProductID = (int)productID,
                ColorID = (int)colorID,
                IconID = (int)iconID,
                IconPath = productImage.Select(pi => new { pi.Image.Icon.icon1 }).FirstOrDefault().icon1,
                ProductDetailID = (int)productImage.Select(c => new { c.ProductDetailID }).FirstOrDefault().ProductDetailID
            };
            return PartialView("Partials/Modals/_EditProductColor", productColorModel);
        }

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

        #region ("START UPDATE PRODUCT AND ITS DETAILS")

        private List<int> CreateNewProductDetail([Bind(Include = "ProductID, ColorID")] ProductColorModel productColorModel, Dictionary<int, int> newSizeAndQuantity)
        {
            List<int> lstNewProductDetailID = new List<int>();
            if (ModelState.IsValid)
            {
                ProductDetail productDetail = new ProductDetail();
                foreach (var item in newSizeAndQuantity)  //loop through on how many new sizes is selected by the user
                {
                    for (int c = 0; c < item.Value; c++)  //on how many size quantity
                    {
                        productDetail.ProductID = productColorModel.ProductID;
                        productDetail.ColorID = productColorModel.ColorID;
                        productDetail.SizeID = item.Key;

                        dbContext.ProductDetail.Add(productDetail);
                        dbContext.SaveChanges();
                        lstNewProductDetailID.Add(dbContext.ProductDetail.Max(pd => pd.ProductDetailID));
                    }
                }
            }
            return lstNewProductDetailID;
        }

        private ActionResult CreateNewProductImage([Bind(Include = "ImageID")] ProductColorModel productColorModel, List<int> lstNewProductDetailID)
        {
            if(ModelState.IsValid)
            {
                ProductImage productImage = new ProductImage();
                foreach (var item in lstNewProductDetailID)
                {
                    foreach (var imgID in productColorModel.ImageID)
                    {
                        productImage.ProductDetailID = item;
                        productImage.ImageID = imgID;
                        productImage.isMainDisplay = false;   //set automatic to false

                        dbContext.ProductImage.Add(productImage);
                        dbContext.SaveChanges();
                    }
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult CreateNewDisplaySize([Bind(Include = "ProductDetailID")] ProductColorModel productColorModel, List<int> lstNewProductDetailID)
        {
            if(ModelState.IsValid)
            {
                DisplaySize displaySize = new DisplaySize();
                int isDisplayImgID = (int)dbContext.ProductImage
                                                   .Where(pi => pi.ProductDetailID == productColorModel.ProductDetailID)
                                                   .Join(dbContext.DisplaySize, pi => pi.ProductImageID, ds => ds.ProductImageID, (pi, ds) => new
                                                   {
                                                       pi.ImageID
                                                   }).FirstOrDefault().ImageID;

                foreach (var item in lstNewProductDetailID)
                {
                    displaySize.ProductImageID = dbContext.ProductImage
                                                          .Where(pi => pi.ProductDetailID == item && pi.ImageID == isDisplayImgID)
                                                          .Select(pi => new { pi.ProductImageID }).FirstOrDefault().ProductImageID;
                    displaySize.isDisplay = false;
                    dbContext.DisplaySize.Add(displaySize);
                    dbContext.SaveChanges();
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult CreateNewProductSize([Bind(Include = "ProductID, ProductDetailID, ColorID, SizeID, ImageID, NewSizeQuantity")] ProductColorModel productColorModel)  //creating/adding new product size in update area
        {
            if(ModelState.IsValid)
            {
                Dictionary<int, int> newSizeAndQuantity = new Dictionary<int, int>();
                foreach(var item in productColorModel.NewSizeQuantity.Where(i => i != "").ToArray())
                {
                    newSizeAndQuantity.Add(Convert.ToInt32(item.Split('-').First()), Convert.ToInt32(item.Split('-').Last()));
                }

                List<int> lstNewProductDetailID = CreateNewProductDetail(productColorModel, newSizeAndQuantity);
                CreateNewProductImage(productColorModel, lstNewProductDetailID);
                CreateNewDisplaySize(productColorModel, lstNewProductDetailID);
                
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "1")]
        public ActionResult EditProductColor(ProductColorModel productColorModel)
        {
            if(ModelState.IsValid)
            {
                if(productColorModel.NewSizeQuantity.Any(s => s != "")) { CreateNewProductSize(productColorModel); }  //check if new size has been added/selected by the admin
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        #endregion ("END UPDATE PRODUCT AND ITS DETAILS")

        #region ("START VIEWING AND MANAGING GALLERY")
        public ActionResult ViewPhotoGallery(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ViewProductModel> productGallery = dbContext.ProductImage
                                                             .Select(i => new ViewProductModel
                                                             {
                                                                ProductImage = i
                                                             }).Where(d => d.ProductImage.ProductDetail.ProductID == productID && d.ProductImage.ProductDetail.ColorID == colorID && d.ProductImage.Image.IconID == iconID).ToList();

            ViewBag.ProductID = productID;
            return PartialView("Partials/Modals/_ProductImageGallery", productGallery);
        }

        [Authorize(Roles = "1")]
        public ActionResult SetImageDisplay(int? defaultDisplayID, int? selectedDisplayID)
        {
            if(defaultDisplayID == null || selectedDisplayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            DisplayColor displayColor = new DisplayColor();
            displayColor.DisplayColorID = dbContext.DisplayColor
                                                   .AsNoTracking()
                                                   .Where(dc => dc.ProductImageID == defaultDisplayID)
                                                   .SingleOrDefault().DisplayColorID;
            displayColor.ProductImageID = selectedDisplayID;
            dbContext.DisplayColor.Attach(displayColor);
            dbContext.Entry(displayColor).Property(dc => dc.ProductImageID).IsModified = true;
            dbContext.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize(Roles = "1")]
        public ActionResult SetImageMainDisplay(int? productID, int? selectedMainDisplayID)
        {
            if(productID == null || selectedMainDisplayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UpdatePreviousMainDisplay(productID);
            SetNewMainDisplay(selectedMainDisplayID);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }



        private ActionResult SetNewMainDisplay(int? selectedMainDisplayID)
        {
            if(selectedMainDisplayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductImage productImage = new ProductImage();
            productImage.ProductImageID = (int)selectedMainDisplayID;
            productImage.isMainDisplay = true;   //setting new main display
            dbContext.ProductImage.Attach(productImage);
            dbContext.Entry(productImage).Property(pi => pi.isMainDisplay).IsModified = true;
            dbContext.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult UpdatePreviousMainDisplay(int? productID)
        {
            if(productID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProductImage productImage = new ProductImage();
            productImage.ProductImageID = dbContext.ProductImage
                                                   .AsNoTracking()
                                                   .Where(pi => pi.ProductDetail.ProductID == productID && pi.isMainDisplay == true)
                                                   .SingleOrDefault().ProductImageID;
            productImage.isMainDisplay = false;
            dbContext.ProductImage.Attach(productImage);
            dbContext.Entry(productImage).Property(pi => pi.isMainDisplay).IsModified = true;
            dbContext.SaveChanges();
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
    }
}