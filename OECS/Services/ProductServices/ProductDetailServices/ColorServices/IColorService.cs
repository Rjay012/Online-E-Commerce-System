using OECS.Models.ProductModels.ProductDetailModels.ColorModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OECS.Services.ProductServices.ProductDetailServices.ColorServices
{
    public interface IColorService
    {
        List<ColorModel> ViewListingColor(int categoryID, int subCategoryID);
        IEnumerable<SelectListItem> ListingColor();
    }
}
