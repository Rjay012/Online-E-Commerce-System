using OECS.Models;
using OECS.Models.ProductModels;
using OECS.Models.ProductModels.ProductDetailModels;
using System.Collections.Generic;

namespace OECS.Repository.ProductRepository
{
    public interface IProductRepository
    {
        List<ViewProductDetailModel> ViewListingNewlyAddedProduct(int? columnToSort, string sortDir);
        List<ViewProductDetailModel> ViewListingProduct(int columnToSort, string sortDir);
        List<ViewProductModel> ShowProductList(int filter);
        bool CreateProduct(Product product);
        bool EditProduct();
        Product FindByID(int productID);
    }
}
