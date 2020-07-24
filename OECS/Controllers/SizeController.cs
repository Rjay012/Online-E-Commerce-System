using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels.SizeModels;
using OECS.Repository.ProductRepository.ProductDetailRepository.SizeRepository;
using OECS.Services.ProductServices.ProductDetailServices.SizeServices;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class SizeController : Controller
    {
        private readonly ISizeService _sizeService;
        public SizeController()
        {
            _sizeService = new SizeService(new SizeRepository(new oecsEntities()));
        }

        // GET: Size
        public ActionResult Index()
        {
            ViewBag.ModuleTitle = "Size";
            return View();
        }

        public ActionResult ShowProductSize(int? categoryID, int? subCategoryID)
        {
            if (categoryID == null || subCategoryID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<SizeModel> sizeModel = _sizeService.ViewListingSize((int)categoryID, (int)subCategoryID);

            if (categoryID != 0) 
            { 
                sizeModel = sizeModel.Where(pd => pd.CategoryID == categoryID).ToList(); 
            }
            
            if (subCategoryID != 0) 
            { 
                sizeModel = sizeModel.Where(pd => pd.SubCategoryID == subCategoryID).ToList(); 
            }

            return PartialView("Partials/_ProductSize", sizeModel);
        }
    }
}