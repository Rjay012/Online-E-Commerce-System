using OECS.Models;
using System.Collections.Generic;

namespace OECS.Repository.CategoryRepository
{
    public interface ICategoryRepository
    {
        List<SubCategory> ListCategory();
    }
}
