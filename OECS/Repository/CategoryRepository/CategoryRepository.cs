using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Repository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly oecsEntities _dbContext;
        public CategoryRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<SelectListItem> ListCategory()
        {
            List<SelectListItem> CategoryListTempStorage = new List<SelectListItem>();
            var subCategory = _dbContext.SubCategory
                                        .OrderBy(c => c.Category.category1)
                                        .ToList();
            foreach (var item in subCategory)
            {
                CategoryListTempStorage.Add(new SelectListItem
                {
                    Value = item.SubCategoryID.ToString(),
                    Text = item.Category.category1 + " • " + item.subCategory1
                });
            }

            return CategoryListTempStorage;
        }
    }
}