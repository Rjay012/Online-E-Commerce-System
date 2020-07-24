using OECS.Models;
using OECS.Models.BrandModels;
using OECS.Repository.BrandRepository;
using OECS.Services.BrandServices;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;
        public BrandController()
        {
            _brandService = new BrandService(new BrandRepository(new oecsEntities()));
        }

        // GET: Brand
        public ActionResult Index()
        {
            ViewBag.ModuleTitle = "Brand";
            return View();
        }

        public ActionResult BrandList(int? categoryID, int? subCategoryID)
        {
            List<BrandModel> brandModel = _brandService.ViewListingBrands((int)categoryID, (int)subCategoryID);

            if (categoryID != 0) 
            { 
                brandModel = brandModel.Where(pd => pd.CategoryID == categoryID)
                                       .ToList(); 
            }

            if (subCategoryID != 0) 
            { 
                brandModel = brandModel.Where(pd => pd.SubCategoryID == subCategoryID)
                                       .ToList(); 
            }
            return PartialView("Partials/_BrandList", brandModel);
        }
    }
}