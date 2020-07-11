using OECS.Models.ProductModels.ProductDetailModels;
using System.Web.Mvc;

namespace OECS.Repository.ProductRepository
{
    public interface IProductDetailRepository
    {
        void CreateProductDetail(ProductDetailModel productDetailModel);
        void EditProductDetail([Bind]ProductDetailModel productDetailModel);
    }
}
