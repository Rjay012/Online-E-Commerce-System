using OECS.Models;
using System.Collections.Generic;
using System.Linq;

namespace OECS.Repository.CategoryRepository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly oecsEntities _dbContext;
        public CategoryRepository(oecsEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SubCategory> ListCategory()
        {
            return _dbContext.SubCategory
                             .OrderBy(c => c.Category.category1)
                             .ToList();
        }
    }
}