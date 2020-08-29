using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels;
using System.Collections.Generic;

namespace OECS.Repository.ProductRepository.ProductGalleryRepository
{
    public interface IProductGalleryRepository
    {
        void UpdatePreviousMainDisplay(int productID);
        void SetNewMainDisplay(int selectedMainDisplayID);
        void SetImageDisplay(int defaultDisplayID, int selectedDisplayID);
        ViewProductDetailModel ViewListingProductImage(int productID, int colorID, int iconID);
        IEnumerable<ProductImage> ProductImageList(int productID, int colorID, int iconID);
        string GetImageDisplayPath(int productID, int colorID);
    }
}
