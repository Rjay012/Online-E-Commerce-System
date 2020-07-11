using OECS.Models;
using OECS.Models.ProductModels;
using OECS.Models.ProductModels.ProductDetailModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
            List<ViewProductDetailModel> product = (from p in dbContext.Product
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
            switch (columnToSort)  //column sorting
            {
                case 0:
                    product = product.OrderByDescending(c => c.Date).ToList();
                    break;
                case 1:
                    product = sortDir == "asc" ? product.OrderBy(c => c.ProductName).ToList() : product.OrderByDescending(c => c.ProductName).ToList();
                    break;
                case 2:
                    product = sortDir == "asc" ? product.OrderBy(c => c.BrandName).ToList() : product.OrderByDescending(c => c.BrandName).ToList();
                    break;
                case 3:
                    product = sortDir == "asc" ? product.OrderBy(c => c.CategoryName).ToList() : product.OrderByDescending(c => c.CategoryName).ToList();
                    break;
                case 5:
                    product = sortDir == "asc" ? product.OrderBy(c => c.Date).ToList() : product.OrderByDescending(c => c.Date).ToList();
                    break;
                case 6:
                    product = sortDir == "asc" ? product.OrderBy(c => c.Price).ToList() : product.OrderByDescending(c => c.Price).ToList();
                    break;
            }
            return product;
        }

        public List<ViewProductDetailModel> ViewListingProduct(int columnToSort, string sortDir)
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
                                                                CategoryName = pi.ProductDetail.Product.SubCategory.Category.category1 + " • " + pi.ProductDetail.Product.SubCategory.subCategory1,
                                                                BrandName = pi.ProductDetail.Product.Brand.BrandName,
                                                                Date = pi.ProductDetail.Product.date,
                                                                Price = pi.ProductDetail.Product.price
                                                            }).Where(pi => pi.isMainDisplay == true).ToList();

            switch (columnToSort)  //column sorting
            {
                case 0:
                    product = product.OrderByDescending(c => c.Date).ToList();
                    break;
                case 4:
                    product = sortDir == "asc" ? product.OrderBy(c => c.ProductName).ToList() : product.OrderByDescending(c => c.ProductName).ToList();
                    break;
                case 5:
                    product = sortDir == "asc" ? product.OrderBy(c => c.BrandName).ToList() : product.OrderByDescending(c => c.BrandName).ToList();
                    break;
                case 6:
                    product = sortDir == "asc" ? product.OrderBy(c => c.CategoryName).ToList() : product.OrderByDescending(c => c.CategoryName).ToList();
                    break;
                case 8:
                    product = sortDir == "asc" ? product.OrderBy(c => c.Date).ToList() : product.OrderByDescending(c => c.Date).ToList();
                    break;
                case 9:
                    product = sortDir == "asc" ? product.OrderBy(c => c.Price).ToList() : product.OrderByDescending(c => c.Price).ToList();
                    break;
            }
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

            if (brandID != 0)
            {
                product = product.Where(p => p.Product.BrandID == brandID).ToList();
            }
            return product;
        }

        

        public bool CreateProduct(ProductModel productModel)
        {
            Product product = new Product();
            product.productName = productModel.ProductName;
            product.SubCategoryID = productModel.SubCategoryID;
            product.BrandID = productModel.BrandID;
            product.date = Convert.ToDateTime(productModel.Date);
            product.price = productModel.Price;
            product.description = productModel.Description;

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
    }
}