using OECS.Models;
using OECS.Models.BrandModels;
using OECS.Repository.BrandRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Services.BrandServices
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IProductDetailRepository _productDetailRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
            _productDetailRepository = new ProductDetailRepository(new oecsEntities());
        }

        public List<BrandModel> ViewListingBrands(int categoryID, int subCategoryID)
        {
            IQueryable<ProductDetail> productDetail = _productDetailRepository.ProductDetail();
            return productDetail.Select(s => new BrandModel
                                {
                                    BrandID = (int)s.Product.BrandID,
                                    BrandName = s.Product.Brand.BrandName,
                                    CategoryID = categoryID == 0 ? null : s.Product.SubCategory.CategoryID,
                                    SubCategoryID = subCategoryID == 0 ? null : s.Product.SubCategoryID
                                }).Distinct()
                                  .ToList();
        }

        public IEnumerable<SelectListItem> ListBrands()
        {
            List<SelectListItem> BrandListTempStorage = new List<SelectListItem>();
            List<Brand> brands = _brandRepository.ListBrands();

            foreach (var item in brands)
            {
                BrandListTempStorage.Add(new SelectListItem
                {
                    Value = item.BrandID.ToString(),
                    Text = item.BrandName
                });
            }

            return BrandListTempStorage;
        }
    }
}