using OECS.Models.CategoryModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OECS.Services.CategoryServices
{
    public interface ICategoryService
    {
        IEnumerable<SelectListItem> ListCategory();
        List<CategoryModel> ViewListingCategory();
        List<SubCategoryModel> ViewListingSubCategory(int categoryID);
    }
}
