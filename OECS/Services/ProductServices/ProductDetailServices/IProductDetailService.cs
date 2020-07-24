using OECS.Models.ProductModels.ProductDetailModels;
using System.Web.Mvc;

namespace OECS.Services.ProductServices.ProductDetailServices
{
    public interface IProductDetailService
    {
        ProductDetailModel GetProductDetail(int productID, int colorID, int iconID);
        void CreateProductDetail(ProductDetailModel productDetailModel);
        void EditProductDetail([Bind]ProductDetailModel productDetailModel);
    }
}
