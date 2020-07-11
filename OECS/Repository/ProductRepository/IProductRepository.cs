using OECS.Models.ProductModels;
using OECS.Models.ProductModels.ProductDetailModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OECS.Repository.ProductRepository
{
    public interface IProductRepository
    {
        List<ViewProductDetailModel> ViewListingNewlyAddedProduct(int? columnToSort, string sortDir);
        List<ViewProductDetailModel> ViewListingProduct(int columnToSort, string sortDir);
        List<ViewProductModel> ShowProductList(int filter);
        List<ViewProductModel> OnFilter(List<ViewProductModel> product, int? categoryID, int? subCategoryID, int? brandID, int? colorID, int? sizeID);
        bool CreateProduct(ProductModel productModel);
        bool EditProduct();
    }
}
