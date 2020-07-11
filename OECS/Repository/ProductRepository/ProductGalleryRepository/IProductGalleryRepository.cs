using OECS.Models;
using System.Collections.Generic;

namespace OECS.Repository.ProductRepository.ProductGalleryRepository
{
    public interface IProductGalleryRepository
    {
        void UpdatePreviousMainDisplay(int? productID);
        void SetNewMainDisplay(int? selectedMainDisplayID);
        void SetImageDisplay(int? defaultDisplayID, int? selectedDisplayID);
        List<ProductImage> ViewListingProductImage(int productID, int colorID, int iconID);
    }
}
