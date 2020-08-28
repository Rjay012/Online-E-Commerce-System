using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using OECS.Repository.ProductRepository.ProductGalleryRepository;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Services.ProductServices.ProductGalleryServices
{
    public class ProductGalleryService : IProductGalleryService
    {
        private readonly IProductGalleryRepository _productGalleryRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        public ProductGalleryService(IProductGalleryRepository productGalleryRepository)
        {
            _productGalleryRepository = productGalleryRepository;
            _productDetailRepository = new ProductDetailRepository(new oecsEntities());
        }

        public IEnumerable<ProductImage> PreviewProductImages(int productID, int colorID, int iconID)
        {
            return _productGalleryRepository.ProductImageList(productID, colorID, iconID);
        }

        public ViewProductDetailModel ViewListingProductImage(int productID, int colorID, int iconID)
        {
            return _productGalleryRepository.ViewListingProductImage(productID, colorID, iconID);
        }

        public void SetImageDisplay(int defaultDisplayID, int selectedDisplayID)
        {
            _productGalleryRepository.SetImageDisplay(defaultDisplayID, selectedDisplayID);
        }

        public void UpdatePreviousMainDisplay(int productID)
        {
            _productGalleryRepository.UpdatePreviousMainDisplay(productID);
        }

        public void SetNewMainDisplay(int selectedMainDisplayID)
        {
            _productGalleryRepository.SetNewMainDisplay(selectedMainDisplayID);
        }

        public IQueryable<ProductDetailModel> GetColorAndIcon(int productID)
        {
            return _productDetailRepository.GetColorAndIcon(productID);
        }

        public string GetImageDisplayPath(int productID)
        {
            return _productGalleryRepository.GetImageDisplayPath(productID);
        }
    }
}