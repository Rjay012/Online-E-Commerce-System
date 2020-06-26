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
using OECS.ClassServices;

namespace OECS.Controllers
{
    public class ProductController : Controller
    {
        readonly oecsEntities dbContext = new oecsEntities();

        // GET: Product
        [Authorize(Roles = "1,2,3")]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Show(int? categoryID, int? subCategoryID, int? brandID, int? colorID, int? sizeID, string searchString)
        {
            _Product _product = new _Product(dbContext);

            List<ViewProductModel> product = _product.ShowProductList(1);
            if (colorID != 0) product = _product.ShowProductList(2);
            if (sizeID != 0) product = _product.ShowProductList(3);

            if (!String.IsNullOrEmpty(searchString)) //search string
            {
                product = product.Where(p => p.Product.SubCategory.subCategory1.Contains(searchString) ||
                                             p.Product.productName.Contains(searchString)).ToList();  //you removed color searching
            }

            product = _product.OnFilter(product, categoryID, subCategoryID, brandID, colorID, sizeID);
            if (colorID != 0 || sizeID != 0) return PartialView("Partials/_ProductList", product);
           
            //PagedList<Product> newProduct = new PagedList<Product>(product, page, pageSize);
            return PartialView("Partials/_ProductList", product.Where(i => i.ProductImage.isMainDisplay == true).ToList());
        }

        #region ("START MODAL FORMS")
        public ActionResult ReadyProductListTable()
        {
            return PartialView("Partials/Tables/_ReadyProductList");
        }

        public ActionResult ShowNewlyAddedProductListTable()
        {
            return PartialView("Partials/Tables/_NewlyAddedProduct");
        }
        public ActionResult NewProductModalForm(ProductModel productModel)
        {
            productModel.Date = DateTime.Now.ToString("yyyy-MM-dd");
            return PartialView("Partials/Modals/_NewProduct", productModel);
        }

        public ActionResult NewProductDetailModalForm(ProductDetailModel productDetailModel)
        {
            return PartialView("Partials/Modals/_NewProductDetail", productDetailModel);
        }

        public ActionResult EditProductDetailModalForm(int? productID, int? colorID, int? iconID)
        {
            if (productID == null && colorID == null && iconID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ProductImage> productImage = dbContext.ProductImage
                                                       .Where(i => i.ProductDetail.ProductID == productID && i.ProductDetail.ColorID == colorID && i.Image.IconID == iconID).ToList();

            ProductDetailModel productDetailModel = new ProductDetailModel
            {
                ProductImage = productImage.ToList(),
                ProductID = (int)productID,
                ColorID = (int)colorID,
                IconID = (int)iconID,
                IconPath = productImage.Select(pi => new { pi.Image.Icon.icon1 }).FirstOrDefault().icon1,
                ProductDetailID = (int)productImage.Select(c => new { c.ProductDetailID }).FirstOrDefault().ProductDetailID
            };
            return PartialView("Partials/Modals/_EditProductDetail", productDetailModel);
        }
        #endregion ("END MODAL FORMS")

        #region ("START CREATE PRODUCT AND ITS DETAILS")
        [HttpPost]
        [Authorize(Roles = "1")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "CategoryList")]ProductModel productModel)
        {
            if(ModelState.IsValid)
            {
                Product product = new Product();
                product.productName = productModel.ProductName;
                product.SubCategoryID = productModel.SubCategoryID;
                product.BrandID = productModel.BrandID;
                product.date = Convert.ToDateTime(productModel.Date);
                product.price = productModel.Price;
                product.description = productModel.Description;
                dbContext.Product.Add(product);
                dbContext.SaveChanges();

                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
            }
            return View(productModel);
        }

        private ActionResult CreateNewIcon([Bind(Include = "IconPath")] ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid)
            {
                Icon icon = new Icon();
                icon.icon1 = productDetailModel.IconPath;
                dbContext.Icon.Add(icon);
                dbContext.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private List<int> CreateProductDetail([Bind(Include = "ProductID, ColorID, SizeID")] ProductDetailModel productDetailModel)
        {
            ProductDetail productDetail = new ProductDetail();
            List<int> lstProductDetailID = new List<int>();
            if (ModelState.IsValid)
            {
                int[] arrSize = productDetailModel.SizeID.Where(s => s != 0).ToArray();
                foreach (int s in arrSize)
                {
                    productDetail.ProductID = productDetailModel.ProductID;
                    productDetail.ColorID = productDetailModel.ColorID;
                    productDetail.SizeID = s;
                    dbContext.ProductDetail.Add(productDetail);
                    dbContext.SaveChanges();

                    lstProductDetailID.Add(dbContext.ProductDetail.Max(d => d.ProductDetailID));
                }
            }
            return lstProductDetailID;
        }

        private Dictionary<int, bool> UploadImage([Bind(Include = "Files, IsDisplayPosition")] ProductDetailModel productDetailModel)
        {
            Image image = new Image();
            Dictionary<int, bool> lstImageID = new Dictionary<int, bool>();
            int position = 1;
            if (ModelState.IsValid)
            {
                foreach (HttpPostedFileBase file in productDetailModel.Files)
                {
                    string fname = UploadFile(file, "/Images");
                    image.path = "Images\\" + fname;
                    image.IconID = dbContext.Icon.Max(i => i.IconID);
                    dbContext.Image.Add(image);
                    dbContext.SaveChanges();

                    if (position == productDetailModel.IsDisplayPosition)
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

        [HttpPost]
        [Authorize(Roles = "1")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewProductDetail(ProductDetailModel productDetailModel)
        {
            if (productDetailModel.IconFile == null || productDetailModel.Files == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                ProductImage productImage = new ProductImage();
                string fname = productDetailModel.Path;

                //upload new icon
                productDetailModel.IconPath = "Images\\AddImageIcon\\" + UploadFile(productDetailModel.IconFile, "/Images/AddImageIcon");
                CreateNewIcon(new ProductDetailModel() { IconPath = productDetailModel.IconPath });
                List<int> lstProductDetailID = CreateProductDetail(new ProductDetailModel { ProductID = productDetailModel.ProductID, ColorID = productDetailModel.ColorID, SizeID = productDetailModel.SizeID });
                Dictionary<int, bool> lstImageID = UploadImage(new ProductDetailModel { Files = productDetailModel.Files, IsDisplayPosition = productDetailModel.IsDisplayPosition });

                foreach (int productDetailID in lstProductDetailID)
                {
                    foreach (int imageID in lstImageID.Keys)
                    {
                        bool findMainDisplay = dbContext.ProductImage
                                                        .Where(pi => pi.ProductDetail.ProductID == productDetailModel.ProductID && pi.isMainDisplay == true).Any();
                        productImage.ProductDetailID = productDetailID;
                        productImage.ImageID = imageID;
                        productImage.isMainDisplay = (findMainDisplay == true ? false : true);  //for each product color, only 1 color can be set to main display

                        dbContext.ProductImage.Add(productImage);
                        dbContext.SaveChanges();
                    }
                }

                //add display color and size
                CreateDisplayDetail(new ProductDetailModel { ProductID = productDetailModel.ProductID }, lstImageID.Where(i => i.Value == true).FirstOrDefault().Key);
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        private ActionResult CreateDisplayColor([Bind(Include = "ProductImageID, ProductID, ColorID")] ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid)
            {
                _ProductService _productService = new _ProductService(dbContext);
                DisplayColor displayColor;
                bool hasColor = _productService.HasColor(productDetailModel.ProductID, productDetailModel.ColorID);  //moved to class _ProductService

                if (hasColor == true)
                {
                    displayColor = new DisplayColor();
                    displayColor.DisplayColorID = _productService.GetDisplayColorKey(productDetailModel.ProductID, productDetailModel.ColorID); //moved to class _ProductService
                    displayColor.isDisplay = false;
                    dbContext.DisplayColor.Attach(displayColor);
                    dbContext.Entry(displayColor).Property(dc => dc.isDisplay).IsModified = true;
                    dbContext.SaveChanges();
                }

                displayColor = new DisplayColor();
                displayColor.ProductImageID = productDetailModel.ProductImageID;
                displayColor.isDisplay = true;
                dbContext.DisplayColor.Add(displayColor);
                dbContext.SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult CreateDisplayDetail([Bind(Include = "ProductID")] ProductDetailModel productDetailModel, int imgID)
        {
            List<ProductDetailModel> productImage = dbContext.ProductImage
                                                            .Where(pi => pi.ImageID == imgID)
                                                            .Select(pi => new ProductDetailModel { ProductImageID = pi.ProductImageID, SID = (int)pi.ProductDetail.SizeID, ColorID = (int)pi.ProductDetail.ColorID }).ToList();
            CreateDisplayColor(new ProductDetailModel() { ProductImageID = productImage.FirstOrDefault().ProductImageID, ProductID = productDetailModel.ProductID, ColorID = productImage.FirstOrDefault().ColorID });
            CreateDisplaySize(productImage, productDetailModel.ProductID);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult CreateDisplaySize(List<ProductDetailModel> productColorList, int? ProductID)
        {
            if(ProductID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            _ProductService _productService = new _ProductService(dbContext);
            DisplaySize displaySize;
            foreach (var i in productColorList)
            {
                if (_productService.HasSize(ProductID, i.SID) == true)
                {
                    displaySize = new DisplaySize();
                    displaySize.DisplaySizeID = _productService.GetDisplaySizeKey(ProductID, i.SID);
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

        private List<int> CreateProductDetail([Bind(Include = "ProductID, ColorID")] ProductDetailModel productDetailModel, Dictionary<int, int> newSizeAndQuantity)
        {
            List<int> lstNewProductDetailID = new List<int>();
            if (ModelState.IsValid)
            {
                ProductDetail productDetail = new ProductDetail();
                foreach (var item in newSizeAndQuantity)  //loop through on how many new sizes is selected by the user
                {
                    for (int c = 0; c < item.Value; c++)  //on how many size quantity
                    {
                        productDetail.ProductID = productDetailModel.ProductID;
                        productDetail.ColorID = productDetailModel.ColorID;
                        productDetail.SizeID = item.Key;

                        dbContext.ProductDetail.Add(productDetail);
                        dbContext.SaveChanges();
                        lstNewProductDetailID.Add(dbContext.ProductDetail.Max(pd => pd.ProductDetailID));
                    }
                }
            }
            return lstNewProductDetailID;
        }

        private ActionResult CreateNewProductImage([Bind(Include = "ImageID")] ProductDetailModel productDetailModel, List<int> lstNewProductDetailID)
        {
            if (ModelState.IsValid)
            {
                ProductImage productImage = new ProductImage();
                foreach (var item in lstNewProductDetailID)
                {
                    foreach (var imgID in productDetailModel.ImageID)
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

        private ActionResult CreateNewDisplaySize([Bind(Include = "ProductDetailID")] ProductDetailModel productDetailModel, List<int> lstNewProductDetailID)
        {
            if (ModelState.IsValid)
            {
                _ProductService _productService = new _ProductService(dbContext);
                DisplaySize displaySize = new DisplaySize();
                int isDisplayImgID = _productService.GetDisplayImageKey(productDetailModel.ProductDetailID);

                foreach (var item in lstNewProductDetailID)
                {
                    displaySize.ProductImageID = _productService.GetProductImageID(item, isDisplayImgID);
                    displaySize.isDisplay = false;
                    dbContext.DisplaySize.Add(displaySize);
                    dbContext.SaveChanges();
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult CreateNewProductSize([Bind(Include = "ProductID, ProductDetailID, ColorID, SizeID, ImageID, NewSizeQuantity")] ProductDetailModel productDetailModel)  //creating/adding new product size in update area
        {
            if (ModelState.IsValid && productDetailModel.NewSizeQuantity.Any(s => s != ""))
            {
                Dictionary<int, int> newSizeAndQuantity = new Dictionary<int, int>();
                foreach (var item in productDetailModel.NewSizeQuantity.Where(i => i != "").ToArray())
                {
                    newSizeAndQuantity.Add(Convert.ToInt32(item.Split('-').First()), Convert.ToInt32(item.Split('-').Last()));
                }

                List<int> lstNewProductDetailID = CreateProductDetail(productDetailModel, newSizeAndQuantity);
                CreateNewProductImage(productDetailModel, lstNewProductDetailID);
                CreateNewDisplaySize(productDetailModel, lstNewProductDetailID);

            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        //YOU STOPPED HERE, YOU CAN CONTROL TO REMOVE THE NUMBER OF QUANTITY BY USING TAKE METHOD 
        private ActionResult RemoveProductSize([Bind(Include = "ToRemoveSizeID, ColorID, ProductID, IconID")] ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in productDetailModel.ToRemoveSizeID.Where(s => s > 0).ToList())
                {
                    var pdToRemove = dbContext.ProductImage
                                              .Where(pi => pi.ProductDetail.SizeID == item && pi.ProductDetail.ColorID == productDetailModel.ColorID && pi.ProductDetail.ProductID == productDetailModel.ProductID && pi.Image.IconID == productDetailModel.IconID)
                                              .Select(pi => new { pi.ProductDetailID }).Distinct().ToList();
                    foreach (var pdID in pdToRemove)
                    {
                        ProductDetail productDetail = dbContext.ProductDetail.Find(pdID.ProductDetailID);
                    }
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductDetail(ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid)
            {
                /** UNCOMMENT THOSE LINES BELOW IF THE UPDATE FEATURE FOR THE PRODUCT DETAIL HAS BEEN FINISHED **/
                CreateNewProductSize(productDetailModel);      //check if new size has been added/selected by the admin
                //RemoveProductSize(productDetailModel);
                ChangeImageIcon(productDetailModel);
                ChangeProductColor(productDetailModel);
                ChangeProductImage(productDetailModel);
                return Json(new { data = "success" }, JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult ChangeProductImage(ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid)
            {
                RemoveOldImage(productDetailModel);
                ChangeImage(productDetailModel);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult ChangeImage([Bind(Include = "Files, FileToRemove")]ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid)
            {
                int[] arrImgID = productDetailModel.FileToRemove.Where(f => f != 0).ToArray();
                int indx = 0;
                foreach (HttpPostedFileBase file in productDetailModel.Files)
                {
                    Image image = dbContext.Image.Find(arrImgID[indx++]);
                    image.path = "Images\\" + UploadFile(file, "/Images");
                    dbContext.SaveChanges();
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult RemoveOldImage([Bind(Include = "FileToRemove")]ProductDetailModel productDetailModel)    //remove old image from directory
        {
            if (ModelState.IsValid)
            {
                foreach (var item in productDetailModel.FileToRemove.Where(f => f != 0).ToArray())
                {
                    Image image = dbContext.Image.Find(item);
                    _File.RemoveFile(Server.MapPath("~\\" + image.path));
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult ChangeProductColor([Bind(Include = "ProductID, ColorID, IconID, NewColorID")]ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid && productDetailModel.NewColorID != 0)
            {
                var productDetailID = dbContext.ProductDetail
                                               .Where(pd => pd.ProductID == productDetailModel.ProductID && pd.ColorID == productDetailModel.ColorID)
                                               .Join(dbContext.ProductImage, pd => pd.ProductDetailID, pi => pi.ProductDetailID, (pd, pi) => new
                                               {
                                                   pd.ProductDetailID,
                                                   pi.Image.IconID
                                               }).Where(pi => pi.IconID == productDetailModel.IconID).Distinct().ToList();

                foreach (var pdID in productDetailID)
                {
                    ProductDetail productDetail = new ProductDetail();
                    productDetail.ProductDetailID = pdID.ProductDetailID;
                    productDetail.ColorID = productDetailModel.NewColorID;
                    dbContext.ProductDetail.Attach(productDetail);
                    dbContext.Entry(productDetail).Property(pd => pd.ColorID).IsModified = true;
                    dbContext.SaveChanges();
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult ChangeImageIcon([Bind(Include = "IconID, IconPath, IconFile")] ProductDetailModel productDetailModel)
        {
            if (ModelState.IsValid)
            {
                _File.RemoveFile(Server.MapPath("~\\" + productDetailModel.IconPath));
                ChangeIconAndDirectoryPath(productDetailModel);   //update the icon inside the directory and its path to the database
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult ChangeIconAndDirectoryPath([Bind(Include = "IconID, IconFile")] ProductDetailModel productDetailModel)
        {
            Icon icon = new Icon();
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = productDetailModel.IconFile;
                string path = "Images\\AddImageIcon\\" + UploadFile(file, "/Images/AddImageIcon");

                icon.IconID = productDetailModel.IconID;
                icon.icon1 = path;  //new path
                dbContext.Entry(icon).State = EntityState.Modified;
                dbContext.SaveChanges();
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
            if (defaultDisplayID == null || selectedDisplayID == null)
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
            if (productID == null || selectedMainDisplayID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UpdatePreviousMainDisplay(productID);
            SetNewMainDisplay(selectedMainDisplayID);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        private ActionResult SetNewMainDisplay(int? selectedMainDisplayID)
        {
            if (selectedMainDisplayID == null)
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
            if (productID == null)
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
            if (ModelState.IsValid)
            {
                _Product _product = new _Product(dbContext);
                var product = param.isNewlyAdded == false ? _product.ViewListingProduct() : _product.ViewListingNewlyAddedProduct();

                if (!String.IsNullOrEmpty(param.sSearch))
                {
                    product = product.Where(p => p.ProductName.ToLower().Contains(param.sSearch) ||
                                                 p.CategoryName.ToLower().Contains(param.sSearch)).ToList();
                }

                switch (param.iSortCol_0)  //column sorting
                {
                    case 0:
                        product = product.OrderByDescending(c => c.Date).ToList();
                        break;
                    case 3:
                        product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.ProductName).ToList() : product.OrderByDescending(c => c.ProductName).ToList();
                        break;
                    case 4:
                        product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.CategoryName).ToList() : product.OrderByDescending(c => c.CategoryName).ToList();
                        break;
                    case 6:
                        product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.Date).ToList() : product.OrderByDescending(c => c.Date).ToList();
                        break;
                    case 7:
                        product = param.sSortDir_0 == "asc" ? product.OrderBy(c => c.Price).ToList() : product.OrderByDescending(c => c.Price).ToList();
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
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }
    }
}