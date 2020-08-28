using OECS.Models;
using OECS.Models.ProductModels;
using OECS.Models.ProductModels.ProductDetailModels;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Repository.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly oecsEntities dbContext;
        public ProductRepository(oecsEntities _dbContext)
        {
            dbContext = _dbContext;
        }

        public List<ViewProductDetailModel> ViewListingNewlyAddedProduct(int? columnToSort, string sortDir)
        {
            return (from p in dbContext.Product
                    where !(dbContext.ProductDetail.Any(e => e.ProductID == p.ProductID))  //show the product that doesn't have any product details yet
                    select new ViewProductDetailModel
                    {
                        ProductID = p.ProductID,
                        ProductName = p.productName,
                        CategoryName = p.SubCategory.Category.category1 + " • " + p.SubCategory.subCategory1,
                        BrandName = p.Brand.BrandName,
                        Date = p.date,
                        Price = p.price
                    }).ToList();
        }

        public List<ViewProductDetailModel> ViewListingProduct(int columnToSort, string sortDir)
        {
            return dbContext.ProductImage
                            .Select(pi => new ViewProductDetailModel
                            {
                                ProductID = pi.ProductDetail.Product.ProductID,
                                ColorID = (int)pi.ProductDetail.ColorID,
                                display = pi.ProductDetail.Product.display,
                                isMainDisplay = pi.isMainDisplay,
                                IconID = pi.Image.IconID,
                                ProductName = pi.ProductDetail.Product.productName,
                                CategoryName = pi.ProductDetail.Product.SubCategory.Category.category1 + " • " + pi.ProductDetail.Product.SubCategory.subCategory1,
                                BrandName = pi.ProductDetail.Product.Brand.BrandName,
                                Date = pi.ProductDetail.Product.date,
                                Price = pi.ProductDetail.Product.price
                            }).Where(pi => pi.isMainDisplay == true).ToList();
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

        public bool CreateProduct(Product product)
        {
            try
            {
                dbContext.Product.Add(product);
                dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool EditProduct()
        {
            return false;
        }

        public Product FindByID(int productID)
        {
            return dbContext.Product.Find(productID);
        }
    }
}