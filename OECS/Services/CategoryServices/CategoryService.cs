using OECS.Models;
using OECS.Models.CategoryModels;
using OECS.Repository.CategoryRepository;
using OECS.Repository.ProductRepository.ProductDetailRepository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductDetailRepository _productDetailRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
            _productDetailRepository = new ProductDetailRepository(new oecsEntities());
        }

        public IEnumerable<SelectListItem> ListCategory()
        {
            List<SelectListItem> CategoryListTempStorage = new List<SelectListItem>();
            List<SubCategory> subCategories = _categoryRepository.ListCategory();

            foreach (var item in subCategories)
            {
                CategoryListTempStorage.Add(new SelectListItem
                {
                    Value = item.SubCategoryID.ToString(),
                    Text = item.Category.category1 + " • " + item.subCategory1
                });
            }

            return CategoryListTempStorage;
        }

        public List<CategoryModel> ViewListingCategory()
        {
            IQueryable<ProductDetail> productDetail = _productDetailRepository.ProductDetail();

            return productDetail.Select(s => new CategoryModel
                                {
                                    CategoryID = s.Product.SubCategory.Category.CategoryID,
                                    Category = s.Product.SubCategory.Category.category1
                                }).Distinct()
                                  .ToList();
        }

        public List<SubCategoryModel> ViewListingSubCategory(int categoryID)
        {
            IQueryable<ProductDetail> productDetail = _productDetailRepository.ProductDetail();
            List<SubCategoryModel> subCategoryModels = new List<SubCategoryModel>();

            if(categoryID != 0)
            {
                productDetail = productDetail.Where(pd => pd.Product.SubCategory.CategoryID == categoryID);
            }

            subCategoryModels = productDetail.Select(s => new SubCategoryModel
            {
                SubCategoryID = s.Product.SubCategory.SubCategoryID,
                SubCategory = s.Product.SubCategory.subCategory1
            }).Distinct().ToList();

            return subCategoryModels;
        }
    }
}