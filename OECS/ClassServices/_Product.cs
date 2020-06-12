using OECS.Models;
using OECS.Models.ProductModels;
using System.Collections.Generic;
using System.Linq;

namespace OECS.ClassServices
{
    public class _Product
    {
        private readonly oecsEntities dbContext;
        public _Product(oecsEntities _dbContext)
        {
            dbContext = _dbContext;
        }

        public List<ViewProductDetailModel> ViewListingNewlyAddedProduct()
        {
            List<ViewProductDetailModel> product = (from p in dbContext.Product
                                                    where !(dbContext.ProductDetail.Any(e => e.ProductID == p.ProductID))  //show the product that doesn't have any product details yet
                                                    select new ViewProductDetailModel
                                                    {
                                                        ProductID = p.ProductID,
                                                        ProductName = p.productName,
                                                        CategoryName = p.Category.category1,
                                                        Date = p.date,
                                                        Price = p.price
                                                    }).ToList();
            return product;
        }

        public List<ViewProductDetailModel> ViewListingProduct()
        {
            List<ViewProductDetailModel> product = dbContext.ProductImage
                                                            .Select(pi => new ViewProductDetailModel
                                                            {
                                                                ProductID = pi.ProductDetail.Product.ProductID,
                                                                ColorID = pi.ProductDetail.ColorID,
                                                                display = pi.ProductDetail.Product.display,
                                                                isMainDisplay = pi.isMainDisplay,
                                                                IconID = pi.Image.IconID,
                                                                ProductName = pi.ProductDetail.Product.productName,
                                                                CategoryName = pi.ProductDetail.Product.Category.category1,
                                                                Date = pi.ProductDetail.Product.date,
                                                                Price = pi.ProductDetail.Product.price
                                                            }).Where(pi => pi.isMainDisplay == true).ToList();
            return product;
        }

        public List<ViewProductModel> ShowProductList(int filter)
        {
            List<ViewProductModel> viewProduct = new List<ViewProductModel>();
            switch (filter)
            {
                case 1:
                    viewProduct = dbContext.ProductImage
                                           .Select(p => new ViewProductModel
                                           {
                                               Product = p.ProductDetail.Product,
                                               ProductImage = p,
                                               ProductDetail = p.ProductDetail
                                           }).ToList();

                    break;
                case 2:
                    viewProduct = dbContext.DisplayColor
                                           .Select(d => new ViewProductModel
                                           {
                                               Product = d.ProductImage.ProductDetail.Product,
                                               ProductDetail = d.ProductImage.ProductDetail,
                                               ProductImage = d.ProductImage,
                                               DisplayColor = d
                                           }).ToList();
                    break;
                case 3:
                    viewProduct = dbContext.DisplaySize
                                           .Select(d => new ViewProductModel
                                           {
                                               Product = d.ProductImage.ProductDetail.Product,
                                               ProductDetail = d.ProductImage.ProductDetail,
                                               ProductImage = d.ProductImage,
                                               DisplaySize = d
                                           }).ToList();
                    break;
            }
            return viewProduct;
        }
    }
}