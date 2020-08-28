using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository.ColorRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository.SizeRepository;
using OECS.Repository.ProductRepository.ProductGalleryRepository;
using OECS.Services.ProductServices.ProductDetailServices.ColorServices;
using OECS.Services.ProductServices.ProductDetailServices.SizeServices;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Services.ProductServices.ProductDetailServices
{
    public class ProductDetailService : IProductDetailService
    {
        private readonly IColorService _colorService;
        private readonly ISizeService _sizeService;

        private readonly IProductDetailRepository _productDetailRepository;
        private readonly IProductGalleryRepository _productGalleryRepository;

        public ProductDetailService(IProductDetailRepository productDetailRepository)
        {
            _productDetailRepository = productDetailRepository;
            _productGalleryRepository = new ProductGalleryRepository(new oecsEntities());

            _colorService = new ColorService(new ColorRepository(new oecsEntities()));
            _sizeService = new SizeService(new SizeRepository(new oecsEntities()));
        }

        public ProductDetailModel GetProductDetail(int productID, int colorID, int iconID)
        {
            List<ProductImage> productImage = _productGalleryRepository.ProductImageList(productID, colorID, iconID).ToList();

            ProductDetailModel productDetailModel = new ProductDetailModel
            {
                ProductImage = productImage.ToList(),
                ProductID = productID,
                ColorID = colorID,
                IconID = iconID,
                SizeList = _sizeService.SizeList(),
                ColorList = _colorService.ListingColor(),
                IconPath = productImage.Select(pi => new { pi.Image.Icon.icon1 }).FirstOrDefault().icon1,
                ProductDetailID = (int)productImage.Select(c => new { c.ProductDetailID }).FirstOrDefault().ProductDetailID
            };
            return productDetailModel;
        }

        public void CreateProductDetail([Bind]ProductDetailModel productDetailModel)
        {
            //upload icon path 
            productDetailModel.IconPath = "Images\\AddImageIcon\\" + FileMng.UploadFile(productDetailModel.IconFile, "/Images/AddImageIcon");
            _productDetailRepository.CreateNewIcon(new ProductDetailModel() { IconPath = productDetailModel.IconPath });
            //create product size and color
            List<int> ListingProductDetailID = _productDetailRepository.CreateProductSizeAndColor(new ProductDetailModel { ProductID = productDetailModel.ProductID, ColorID = productDetailModel.ColorID, SizeID = productDetailModel.SizeID });
            //upload images
            Dictionary<int, bool> ListingImageID = _productDetailRepository.UploadImage(new ProductDetailModel { Files = productDetailModel.Files, IsDisplayPosition = productDetailModel.IsDisplayPosition });

            _productDetailRepository.CreateProductDetail(productDetailModel, ListingProductDetailID, ListingImageID);
            _productDetailRepository.CreateDisplayDetail(new ProductDetailModel { ProductID = productDetailModel.ProductID }, ListingImageID.Where(i => i.Value == true).FirstOrDefault().Key);
        }

        public void EditProductDetail([Bind]ProductDetailModel productDetailModel)
        {
            /** UNCOMMENT THOSE LINES BELOW IF THE UPDATE FEATURE FOR THE PRODUCT DETAIL HAS BEEN FINISHED **/
            _productDetailRepository.CreateNewProductSize(productDetailModel);      //check if new size has been added/selected by the admin
            //_productDetailRepository.RemoveProductSize(productDetailModel);
            _productDetailRepository.ChangeImageIcon(productDetailModel);
            _productDetailRepository.ChangeProductColor(productDetailModel);
            _productDetailRepository.ChangeProductImage(productDetailModel);
        }

        public IEnumerable<ProductDetailModel> ProductDetailList(ViewProductDetailModel viewProductDetailModel)
        {
            return _productDetailRepository.ProductDetailList(viewProductDetailModel)
                                           .Select(s => new ProductDetailModel
                                           {
                                               ProductDetailID = s.ProductDetailID
                                           }).ToList();
        }
    }
}