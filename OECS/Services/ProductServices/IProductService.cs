using OECS.Models;
using OECS.Models.ProductModels;
using OECS.Models.ProductModels.ProductDetailModels;
using System.Collections.Generic;

namespace OECS.Services.ProductServices
{
    public interface IProductService
    {
        List<ViewProductDetailModel> ViewListingNewlyAddedProduct(int? columnToSort, string sortDir);
        List<ViewProductDetailModel> ViewListingProduct(int columnToSort, string sortDir);
        List<ViewProductModel> ShowProductList(int filter);
        ProductModel AssignProductDetail(ProductModel productModel);
        ProductDetailModel AssignProductDetail(ProductDetailModel productDetailModel);
        List<ViewProductModel> OnFilter(List<ViewProductModel> product, int? categoryID, int? subCategoryID, int? brandID, int? colorID, int? sizeID);
        bool CreateProduct(ProductModel productModel);
        Product FindByID(int productID);
        ProductModel AssignProduct(Product product);
    }
}
