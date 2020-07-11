using OECS.Models.BrandModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OECS.Repository.BrandRepository
{
    public interface IBrandRepository
    {
        List<BrandModel> ViewListingBrands(int categoryID, int subCategoryID);
        IEnumerable<SelectListItem> ListBrands();
    }
}
