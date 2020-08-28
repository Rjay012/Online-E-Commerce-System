using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Services.ProductServices.ProductGalleryServices
{
    public interface IProductGalleryService
    {
        ViewProductDetailModel ViewListingProductImage(int productID, int colorID, int iconID);
        void SetImageDisplay(int defaultDisplayID, int selectedDisplayID);
        void UpdatePreviousMainDisplay(int productID);
        void SetNewMainDisplay(int selectedMainDisplayID);
        IQueryable<ProductDetailModel> GetColorAndIcon(int productID);
        IEnumerable<ProductImage> PreviewProductImages(int productID, int colorID, int iconID);
        string GetImageDisplayPath(int productID);
    }
}
