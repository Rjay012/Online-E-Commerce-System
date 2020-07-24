using OECS.Models.BrandModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OECS.Services.BrandServices
{
    public interface IBrandService
    {
        List<BrandModel> ViewListingBrands(int categoryID, int subCategoryID);
        IEnumerable<SelectListItem> ListBrands();
    }
}
