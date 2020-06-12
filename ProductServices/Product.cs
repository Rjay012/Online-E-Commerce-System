using OECS.Models;
using OECS.Models.ProductModels;
using System;
using System.Collections.Generic;

namespace ProductServices
{
    public class Product
    {
        private readonly oecsEntities dbContext;
        public Product(oecsEntities _dbContext)
        {
            dbContext = _dbContext;
        }

        public List<ViewProductDetailModel> ViewProduct()
        {
            List<ViewProductDetailModel> newlyAddedProduct = (from p in dbContext.Product
                                                              where !(dbContext.ProductDetail.Any(e => e.ProductID == p.ProductID))  //show the product that doesn't have any product details yet
                                                              select new ViewProductDetailModel
                                                              {
                                                                  ProductID = p.ProductID,
                                                                  ProductName = p.productName,
                                                                  CategoryName = p.Category.category1,
                                                                  Date = p.date,
                                                                  Price = p.price
                                                              }).ToList();
        }
    }
}
