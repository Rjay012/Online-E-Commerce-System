using OECS.Models;
using OECS.Models.ProductModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.ClassServices
{
    public class _Product
    {
        protected oecsEntities dbContext;
        public _Product(oecsEntities _dbContext)
        {
            dbContext = _dbContext;
        }

        public _Product()
        {
            //default constructor
        }

        public List<ViewProductDetailModel> ViewListingNewlyAddedProduct()
        {
            List<ViewProductDetailModel> product = (from p in dbContext.Product
                                                    where !(dbContext.ProductDetail.Any(e => e.ProductID == p.ProductID))  //show the product that doesn't have any product details yet
                                                    select new ViewProductDetailModel
                                                    {
                                                        ProductID = p.ProductID,
                                                        ProductName = p.productName,
                                                        CategoryName = p.SubCategory.subCategory1,
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
                                                                CategoryName = pi.ProductDetail.Product.SubCategory.subCategory1,
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

        public List<ViewProductModel> OnFilter(List<ViewProductModel> product, int? categoryID, int? subCategoryID, int? brandID, int? colorID, int? sizeID)
        {
            if (categoryID != 0 && subCategoryID == 0)
            {
                product = product.Where(p => p.Product.SubCategory.Category.CategoryID == categoryID).ToList();
            }
            else if (categoryID == 0 && subCategoryID != 0)
            {
                product = product.Where(p => p.Product.SubCategoryID == subCategoryID).ToList();
            }
            else if (categoryID != 0 && subCategoryID != 0)
            {
                product = product.Where(p => p.Product.SubCategory.CategoryID == categoryID && p.Product.SubCategoryID == subCategoryID).ToList();
            }

            return OnFilter(product, brandID, colorID, sizeID);
        }

        private List<ViewProductModel> OnFilter(List<ViewProductModel> product, int? brandID, int? colorID, int? sizeID)
        {
            if (colorID != 0 && sizeID == 0)
            {
                product = product.Where(p => p.ProductDetail.ColorID == colorID && p.DisplayColor.isDisplay == true).ToList();
            }
            else if (colorID == 0 && sizeID != 0)
            {
                product = product.Where(p => p.ProductDetail.SizeID == sizeID && p.DisplaySize.isDisplay == true).ToList();
            }

            if(brandID != 0)
            {
                product = product.Where(p => p.Product.BrandID == brandID).ToList();
            }
            return product;
        }
    }
}