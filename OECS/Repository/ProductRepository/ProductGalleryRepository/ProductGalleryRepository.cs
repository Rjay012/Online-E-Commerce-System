using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Repository.ProductRepository.ProductGalleryRepository
{
    public class ProductGalleryRepository : IProductGalleryRepository
    {
        private readonly oecsEntities _dbContext;
        public ProductGalleryRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public void UpdatePreviousMainDisplay(int productID)
        {
            ProductImage productImage = new ProductImage();
            productImage.ProductImageID = _dbContext.ProductImage
                                                    .AsNoTracking()
                                                    .Where(pi => pi.ProductDetail.ProductID == productID && pi.isMainDisplay == true)
                                                    .SingleOrDefault().ProductImageID;
            productImage.isMainDisplay = false;
            _dbContext.ProductImage.Attach(productImage);
            _dbContext.Entry(productImage).Property(pi => pi.isMainDisplay).IsModified = true;
            _dbContext.SaveChanges();
        }

        public void SetNewMainDisplay(int selectedMainDisplayID)
        {
            ProductImage productImage = new ProductImage();
            productImage.ProductImageID = selectedMainDisplayID;
            productImage.isMainDisplay = true;   //setting new main display
            _dbContext.ProductImage.Attach(productImage);
            _dbContext.Entry(productImage).Property(pi => pi.isMainDisplay).IsModified = true;
            _dbContext.SaveChanges();
        }

        public void SetImageDisplay(int defaultDisplayID, int selectedDisplayID)
        {
            DisplayColor displayColor = new DisplayColor();
            displayColor.DisplayColorID = _dbContext.DisplayColor
                                                    .AsNoTracking()
                                                    .Where(dc => dc.ProductImageID == defaultDisplayID)
                                                    .SingleOrDefault().DisplayColorID;
            displayColor.ProductImageID = selectedDisplayID;
            _dbContext.DisplayColor.Attach(displayColor);
            _dbContext.Entry(displayColor).Property(dc => dc.ProductImageID).IsModified = true;
            _dbContext.SaveChanges();
        }

        public ViewProductDetailModel ViewListingProductImage(int productID, int colorID, int iconID)
        {
            ViewProductDetailModel productDetail = _dbContext.Product
                                                             .Where(p => p.ProductID == productID)
                                                             .Select(s => new ViewProductDetailModel
                                                             {
                                                                 ProductID = productID,
                                                                 ProductName = s.productName,
                                                                 BrandName = s.Brand.BrandName,
                                                                 Description = s.description,
                                                                 Price = s.price,
                                                                 IconID = iconID,
                                                                 ColorID = colorID
                                                             }).FirstOrDefault();
            return productDetail;
        }

        public IEnumerable<ProductImage> ProductImageList(int productID, int colorID, int iconID)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetail.ProductID == productID && pi.ProductDetail.ColorID == colorID && pi.Image.IconID == iconID);
        }

        public string GetImageDisplayPath(int productID, int colorID)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetail.ProductID == productID && pi.ProductDetail.ColorID == colorID)
                             .Join(_dbContext.DisplayColor, pi => pi.ProductImageID, dc => dc.ProductImageID, (pi, dc) => new
                             {
                                 pi.Image.path,
                                 dc.isDisplay
                             })
                             .Where(dc => dc.isDisplay == true)
                             .FirstOrDefault().path;
        }
    }
}