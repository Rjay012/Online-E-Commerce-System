using OECS.Models;
using OECS.Models.BrandModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Repository.BrandRepository
{
    public class BrandRepository : IBrandRepository
    {
        private readonly oecsEntities _dbContext;
        public BrandRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public List<BrandModel> ViewListingBrands(int categoryID, int subCategoryID)
        {
            return _dbContext.ProductDetail
                             .Select(s => new BrandModel
                             {
                                 BrandID = (int)s.Product.BrandID,
                                 BrandName = s.Product.Brand.BrandName,
                                 CategoryID = categoryID == 0 ? null : s.Product.SubCategory.CategoryID,
                                 SubCategoryID = subCategoryID == 0 ? null : s.Product.SubCategoryID
                             }).Distinct().ToList();
        }

        public IEnumerable<SelectListItem> ListBrands()
        {
            List<SelectListItem> BrandListTempStorage = new List<SelectListItem>();
            var subCategory = _dbContext.Brand.ToList();
            foreach (var item in subCategory)
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