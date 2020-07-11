using OECS.Models;
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

        public void UpdatePreviousMainDisplay(int? productID)
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

        public void SetNewMainDisplay(int? selectedMainDisplayID)
        {
            ProductImage productImage = new ProductImage();
            productImage.ProductImageID = (int)selectedMainDisplayID;
            productImage.isMainDisplay = true;   //setting new main display
            _dbContext.ProductImage.Attach(productImage);
            _dbContext.Entry(productImage).Property(pi => pi.isMainDisplay).IsModified = true;
            _dbContext.SaveChanges();
        }

        public void SetImageDisplay(int? defaultDisplayID, int? selectedDisplayID)
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

        public List<ProductImage> ViewListingProductImage(int productID, int colorID, int iconID)
        {
            return _dbContext.ProductImage
                             .Where(pi => pi.ProductDetail.ProductID == productID && pi.ProductDetail.ColorID == colorID && pi.Image.IconID == iconID).ToList();
        }
    }
}