using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Repository.ProductRepository.ProductDetailRepository
{
    public class ProductDetailRepository : IProductDetailRepository
    {
        private readonly oecsEntities _dbContext;
        public ProductDetailRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        #region START ADD PRODUCT DETAIL
        public void CreateProductDetail(ProductDetailModel productDetailModel, List<int> lstProductDetailID, Dictionary<int, bool> lstImageID)
        {
            ProductImage productImage = new ProductImage();

            foreach (int productDetailID in lstProductDetailID)
            {
                foreach (int imageID in lstImageID.Keys)
                {
                    bool isFoundMainDisplay = FindMainDisplay(productDetailModel);
                    productImage.ProductDetailID = productDetailID;
                    productImage.ImageID = imageID;
                    productImage.isMainDisplay = (isFoundMainDisplay == true ? false : true);  //for each product color, only 1 color can be set to main display

                    _dbContext.ProductImage.Add(productImage);
                    _dbContext.SaveChanges();
                }
            }
        }

        private bool FindMainDisplay([Bind(Include = "ProductID")]ProductDetailModel productDetailModel)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetail.ProductID == productDetailModel.ProductID && pi.isMainDisplay == true)
                             .Any();
        }

        public bool CreateNewIcon([Bind(Include = "IconPath")] ProductDetailModel productDetailModel)
        {
            Icon icon = new Icon();
            icon.icon1 = productDetailModel.IconPath;
            try
            {
                _dbContext.Icon.Add(icon);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<int> CreateProductSizeAndColor([Bind(Include = "ProductID, ColorID, SizeID")] ProductDetailModel productDetailModel)
        {
            ProductDetail productDetail = new ProductDetail();
            List<int> lstProductDetailID = new List<int>();
            int[] arrSize = productDetailModel.SizeID.Where(s => s != 0).ToArray();
            foreach (int s in arrSize)
            {
                productDetail.ProductID = productDetailModel.ProductID;
                productDetail.ColorID = productDetailModel.ColorID;
                productDetail.SizeID = s;
                _dbContext.ProductDetail.Add(productDetail);
                _dbContext.SaveChanges();

                lstProductDetailID.Add(_dbContext.ProductDetail.Max(d => d.ProductDetailID));
            }
            return lstProductDetailID;
        }

        public Dictionary<int, bool> UploadImage([Bind(Include = "Files, IsDisplayPosition")] ProductDetailModel productDetailModel)
        {
            Image image = new Image();
            Dictionary<int, bool> lstImageID = new Dictionary<int, bool>();
            int position = 1;
            foreach (HttpPostedFileBase file in productDetailModel.Files)
            {
                string fname = FileMng.UploadFile(file, "/Images");
                image.path = "Images\\" + fname;
                image.IconID = _dbContext.Icon.Max(i => i.IconID);
                _dbContext.Image.Add(image);
                _dbContext.SaveChanges();

                if (position == productDetailModel.IsDisplayPosition)
                {
                    lstImageID.Add(_dbContext.Image.Max(i => i.ImageID), true);  //set us display
                }
                else
                {
                    lstImageID.Add(_dbContext.Image.Max(i => i.ImageID), false);
                }
                position++;
            }
            return lstImageID;
        }

        public void CreateDisplayDetail([Bind(Include = "ProductID")] ProductDetailModel productDetailModel, int imgID)
        {
            List<ProductDetailModel> productImage = _dbContext.ProductImage
                                                              .Where(pi => pi.ImageID == imgID)
                                                              .Select(pi => new ProductDetailModel 
                                                              { 
                                                                  ProductImageID = pi.ProductImageID, 
                                                                  SID = (int)pi.ProductDetail.SizeID, 
                                                                  ColorID = (int)pi.ProductDetail.ColorID 
                                                              }).ToList();

            CreateDisplayColor(new ProductDetailModel() { ProductImageID = productImage.FirstOrDefault().ProductImageID, ProductID = productDetailModel.ProductID, ColorID = productImage.FirstOrDefault().ColorID });
            CreateDisplaySize(productImage, productDetailModel.ProductID);
        }

        private void CreateDisplayColor([Bind(Include = "ProductImageID, ProductID, ColorID")] ProductDetailModel productDetailModel)
        {
            DisplayColor displayColor;
            bool hasColor = HasColor(productDetailModel.ProductID, productDetailModel.ColorID); 

            if (hasColor == true)
            {
                displayColor = new DisplayColor();
                displayColor.DisplayColorID = GetDisplayColorKey(productDetailModel.ProductID, productDetailModel.ColorID); 
                displayColor.isDisplay = false;
                _dbContext.DisplayColor.Attach(displayColor);
                _dbContext.Entry(displayColor).Property(dc => dc.isDisplay).IsModified = true;
                _dbContext.SaveChanges();
            }

            displayColor = new DisplayColor();
            displayColor.ProductImageID = productDetailModel.ProductImageID;
            displayColor.isDisplay = true;
            _dbContext.DisplayColor.Add(displayColor);
            _dbContext.SaveChanges();
        }

        private void CreateDisplaySize(List<ProductDetailModel> productColorList, int? ProductID)
        {
            DisplaySize displaySize;
            foreach (var i in productColorList)
            {
                if (HasSize(ProductID, i.SID) == true)
                {
                    displaySize = new DisplaySize();
                    displaySize.DisplaySizeID = GetDisplaySizeKey(ProductID, i.SID);
                    displaySize.isDisplay = false;
                    _dbContext.DisplaySize.Attach(displaySize);
                    _dbContext.Entry(displaySize).Property(ds => ds.isDisplay).IsModified = true;
                    _dbContext.SaveChanges();
                }
                displaySize = new DisplaySize();
                displaySize.ProductImageID = i.ProductImageID;
                displaySize.isDisplay = true;
                _dbContext.DisplaySize.Add(displaySize);
                _dbContext.SaveChanges();
            }
        }
        #endregion END ADD PRODUCT DETAIL

        #region START EDIT PRODUCT DETAIL

        //YOU STOPPED HERE, YOU CAN CONTROL TO REMOVE THE NUMBER OF QUANTITY BY USING TAKE METHOD 
        public void RemoveProductSize([Bind(Include = "ToRemoveSizeID, ColorID, ProductID, IconID")] ProductDetailModel productDetailModel)
        {
            foreach (var item in productDetailModel.ToRemoveSizeID.Where(s => s > 0).ToList())
            {
                List<ProductDetailModel> ListingProductImageID = GetProductImageID(productDetailModel, item);
                foreach (var pdID in ListingProductImageID)
                {
                    ProductDetail productDetail = _dbContext.ProductDetail.Find(pdID.ProductDetailID);
                }
            }
        }

        private List<ProductDetailModel> GetProductImageID([Bind(Include = "ToRemoveSizeID, ColorID, ProductID, IconID")] ProductDetailModel productDetailModel, int sizeID)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetail.SizeID == sizeID && pi.ProductDetail.ColorID == productDetailModel.ColorID && pi.ProductDetail.ProductID == productDetailModel.ProductID && pi.Image.IconID == productDetailModel.IconID)
                             .Select(pi => new ProductDetailModel { ProductDetailID = pi.ProductDetailID })
                             .Distinct()
                             .ToList();
        }

        public void CreateNewProductSize([Bind(Include = "ProductID, ProductDetailID, ColorID, SizeID, ImageID, NewSizeQuantity")] ProductDetailModel productDetailModel)  //creating/adding new product size in update area
        {
            if (productDetailModel.NewSizeQuantity.Any(s => s != ""))
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
        }

        private void CreateNewDisplaySize([Bind(Include = "ProductDetailID")] ProductDetailModel productDetailModel, List<int> lstNewProductDetailID)
        {
            DisplaySize displaySize = new DisplaySize();
            int isDisplayImgID = GetDisplayImageKey(productDetailModel.ProductDetailID);

            foreach (var item in lstNewProductDetailID)
            {
                displaySize.ProductImageID = GetProductImageID(item, isDisplayImgID);
                displaySize.isDisplay = false;
                _dbContext.DisplaySize.Add(displaySize);
                _dbContext.SaveChanges();
            }
        }

        private void CreateNewProductImage([Bind(Include = "ImageID")] ProductDetailModel productDetailModel, List<int> lstNewProductDetailID)
        {
            ProductImage productImage = new ProductImage();
            foreach (var item in lstNewProductDetailID)
            {
                foreach (var imgID in productDetailModel.ImageID)
                {
                    productImage.ProductDetailID = item;
                    productImage.ImageID = imgID;
                    productImage.isMainDisplay = false;   //set automatic to false

                    _dbContext.ProductImage.Add(productImage);
                    _dbContext.SaveChanges();
                }
            }
        }

        private List<int> CreateProductDetail([Bind(Include = "ProductID, ColorID")] ProductDetailModel productDetailModel, Dictionary<int, int> newSizeAndQuantity)
        {
            List<int> lstNewProductDetailID = new List<int>();
            ProductDetail productDetail = new ProductDetail();
            foreach (var item in newSizeAndQuantity)  //loop through on how many new sizes is selected by the user
            {
                for (int c = 0; c < item.Value; c++)  //on how many size quantity
                {
                    productDetail.ProductID = productDetailModel.ProductID;
                    productDetail.ColorID = productDetailModel.ColorID;
                    productDetail.SizeID = item.Key;

                    _dbContext.ProductDetail.Add(productDetail);
                    _dbContext.SaveChanges();
                    lstNewProductDetailID.Add(_dbContext.ProductDetail.Max(pd => pd.ProductDetailID));
                }
            }
            return lstNewProductDetailID;
        }

        public void ChangeImageIcon([Bind(Include = "IconID, IconPath, IconFile")] ProductDetailModel productDetailModel)
        {
            FileMng.RemoveFile(System.Web.HttpContext.Current.Server.MapPath("~\\" + productDetailModel.IconPath));
            ChangeIconAndDirectoryPath(productDetailModel);   //update the icon inside the directory and its path to the database
        }

        private void ChangeIconAndDirectoryPath([Bind(Include = "IconID, IconFile")] ProductDetailModel productDetailModel)
        {
            Icon icon = new Icon();
            HttpPostedFileBase file = productDetailModel.IconFile;
            string path = "Images\\AddImageIcon\\" + FileMng.UploadFile(file, "/Images/AddImageIcon");

            icon.IconID = productDetailModel.IconID;
            icon.icon1 = path;  //new path
            _dbContext.Entry(icon).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public void ChangeProductColor([Bind(Include = "ProductID, ColorID, IconID, NewColorID")]ProductDetailModel productDetailModel)
        {
            if (productDetailModel.NewColorID != 0)
            {
                List<ProductDetailModel> getProductDetailModel = GetProductDetail(productDetailModel);

                foreach (var item in getProductDetailModel)
                {
                    ProductDetail productDetail = new ProductDetail();
                    productDetail.ProductDetailID = item.ProductDetailID;
                    productDetail.ColorID = productDetailModel.NewColorID;
                    _dbContext.ProductDetail.Attach(productDetail);
                    _dbContext.Entry(productDetail).Property(pd => pd.ColorID).IsModified = true;
                    _dbContext.SaveChanges();
                }
            }
        }

        private List<ProductDetailModel> GetProductDetail(ProductDetailModel productDetailModel)
        {
            return _dbContext.ProductDetail
                             .Where(pd => pd.ProductID == productDetailModel.ProductID && pd.ColorID == productDetailModel.ColorID)
                             .Join(_dbContext.ProductImage, pd => pd.ProductDetailID, pi => pi.ProductDetailID, (pd, pi) => new ProductDetailModel
                             {
                                 ProductDetailID = pd.ProductDetailID,
                                 IconID = (int)pi.Image.IconID
                             })
                             .Where(pi => pi.IconID == productDetailModel.IconID)
                             .Distinct()
                             .ToList();
        }

        public void ChangeProductImage(ProductDetailModel productDetailModel)
        {
            RemoveOldImage(productDetailModel);
            ChangeImage(productDetailModel);
        }

        private void RemoveOldImage([Bind(Include = "FileToRemove")]ProductDetailModel productDetailModel)    //remove old image from directory
        {
            foreach (var item in productDetailModel.FileToRemove.Where(f => f != 0).ToArray())
            {
                Image image = _dbContext.Image.Find(item);
                FileMng.RemoveFile(System.Web.HttpContext.Current.Server.MapPath("~\\" + image.path));
            }
        }

        private void ChangeImage([Bind(Include = "Files, FileToRemove")]ProductDetailModel productDetailModel)
        {
            int[] arrImgID = productDetailModel.FileToRemove.Where(f => f != 0).ToArray();
            int indx = 0;
            foreach (HttpPostedFileBase file in productDetailModel.Files)
            {
                Image image = _dbContext.Image.Find(arrImgID[indx++]);
                image.path = "Images\\" + FileMng.UploadFile(file, "/Images");
                _dbContext.SaveChanges();
            }
        }
        #endregion END EDIT PRODUCT DETAIL

        private bool HasColor(int? productID, int? colorID)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetail.ProductID == productID && pi.ProductDetail.ColorID == colorID)
                             .Join(_dbContext.DisplayColor, pi => pi.ProductImageID, dc => dc.ProductImageID, (pi, dc) => new
                             {
                                 dc.ProductImageID
                             }).Any();
        }

        private int GetDisplayColorKey(int? productID, int? colorID)
        {
            return _dbContext.DisplayColor
                             .Where(dc => dc.ProductImage.ProductDetail.ProductID == productID && dc.ProductImage.ProductDetail.ColorID == colorID && dc.isDisplay == true)
                             .Join(_dbContext.ProductImage, dc => dc.ProductImageID, pi => pi.ProductImageID, (dc, pi) => new
                             {
                                 dc.DisplayColorID
                             }).FirstOrDefault().DisplayColorID;
        }

        private bool HasSize(int? productID, int? sizeID)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetail.ProductID == productID && pi.ProductDetail.SizeID == sizeID)
                             .Join(_dbContext.DisplaySize, pi => pi.ProductImageID, ds => ds.ProductImageID, (pi, ds) => new
                             {
                                 ds.ProductImageID
                             })
                            .Any();
        }

        private int GetDisplaySizeKey(int? productID, int? sizeID)
        {
            return _dbContext.DisplaySize
                             .Where(ds => ds.ProductImage.ProductDetail.ProductID == productID && ds.ProductImage.ProductDetail.SizeID == sizeID && ds.isDisplay == true)
                             .Select(ds => new
                             {
                                 ds.DisplaySizeID
                             }).FirstOrDefault().DisplaySizeID;
        }

        private int GetDisplayImageKey(int? productDetailID)
        {
            return (int)_dbContext.ProductImage
                                  .Where(pi => pi.ProductDetailID == productDetailID)
                                  .Join(_dbContext.DisplaySize, pi => pi.ProductImageID, ds => ds.ProductImageID, (pi, ds) => new
                                  {
                                      pi.ImageID
                                  })
                                 .FirstOrDefault().ImageID;
        }

        private int GetProductImageID(int? productDetailID, int? imageID)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetailID == productDetailID && pi.ImageID == imageID)
                             .Select(pi => new
                             {
                                 pi.ProductImageID
                             })
                            .FirstOrDefault().ProductImageID;
        }

        public IQueryable<ProductDetailModel> GetColorAndIcon(int productID)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetail.ProductID == productID && pi.isMainDisplay == true)
                             .Select(s => new ProductDetailModel
                             {
                                 ColorID = (int)s.ProductDetail.ColorID,
                                 IconID = (int)s.Image.IconID
                             });
        }

        public IQueryable<ProductDetail> ProductDetail()
        {
            return _dbContext.ProductDetail;
        }
    }
}