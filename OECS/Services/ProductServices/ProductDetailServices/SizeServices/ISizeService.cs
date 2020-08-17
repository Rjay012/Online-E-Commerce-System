using OECS.Models.ProductModels.ProductDetailModels.SizeModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace OECS.Services.ProductServices.ProductDetailServices.SizeServices
{
    public interface ISizeService
    {
        List<SizeModel> ViewListingSize(int categoryID, int subCategoryID);
        IEnumerable<SelectListItem> SizeList();
        IEnumerable<SelectListItem> SizeList(int productID, int colorID, int iconID);
    }
}
