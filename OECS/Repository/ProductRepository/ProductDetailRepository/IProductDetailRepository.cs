using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Repository.ProductRepository.ProductDetailRepository
{
    public interface IProductDetailRepository
    {
        void CreateProductDetail(ProductDetailModel productDetailModel, List<int> lstProductDetailID, Dictionary<int, bool> lstImageID);
        bool CreateNewIcon([Bind(Include = "IconPath")] ProductDetailModel productDetailModel);
        List<int> CreateProductSizeAndColor([Bind(Include = "ProductID, ColorID, SizeID")] ProductDetailModel productDetailModel);
        Dictionary<int, bool> UploadImage([Bind(Include = "Files, IsDisplayPosition")] ProductDetailModel productDetailModel);
        void CreateDisplayDetail([Bind(Include = "ProductID")] ProductDetailModel productDetailModel, int imgID);

        void CreateNewProductSize([Bind(Include = "ProductID, ProductDetailID, ColorID, SizeID, ImageID, NewSizeQuantity")] ProductDetailModel productDetailModel);
        void RemoveProductSize([Bind(Include = "ToRemoveSizeID, ColorID, ProductID, IconID")] ProductDetailModel productDetailModel);
        void ChangeImageIcon([Bind(Include = "IconID, IconPath, IconFile")] ProductDetailModel productDetailModel);
        void ChangeProductColor([Bind(Include = "ProductID, ColorID, IconID, NewColorID")]ProductDetailModel productDetailModel);
        void ChangeProductImage(ProductDetailModel productDetailModel);
        IQueryable<ProductDetailModel> GetColorAndIcon(int productID);
        IQueryable<ProductDetail> ProductDetail();
        List<ProductDetailModel> ViewProductIcon(int productID);
    }
}
