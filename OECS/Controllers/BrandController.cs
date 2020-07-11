using OECS.Models;
using OECS.Models.BrandModels;
using OECS.Repository.BrandRepository;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandRepository _brandRepository;
        public BrandController()
        {
            _brandRepository = new BrandRepository(new oecsEntities());
        }

        // GET: Brand
        public ActionResult Index()
        {
            ViewBag.ModuleTitle = "Brand";
            return View();
        }

        public ActionResult BrandList(int? categoryID, int? subCategoryID)
        {
            List<BrandModel> brandModel = _brandRepository.ViewListingBrands((int)categoryID, (int)subCategoryID);

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