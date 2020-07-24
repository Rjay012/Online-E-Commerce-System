using OECS.Models;
using OECS.Models.ProductModels.ProductDetailModels.ColorModels;
using OECS.Repository.ProductRepository.ProductDetailRepository.ColorRepository;
using OECS.Services.ProductServices.ProductDetailServices.ColorServices;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OECS.Controllers
{
    public class ColorController : Controller
    {
        private readonly IColorService _colorService;
        public ColorController()
        {
            _colorService = new ColorService(new ColorRepository(new oecsEntities())); 
        }

        // GET: Color
        public ActionResult Index()
        {
            ViewBag.ModuleTitle = "Size";
            return View();
        }

        public ActionResult ShowProductColor(int? categoryID, int? subCategoryID)
        {
            if(categoryID == null || subCategoryID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<ColorModel> colorModel = _colorService.ViewListingColor((int)categoryID, (int)subCategoryID);

            if (categoryID != 0) 
            { 
                colorModel = colorModel.Where(pd => pd.CategoryID == categoryID).ToList(); 
            }

            if (subCategoryID != 0) 
            { 
                colorModel = colorModel.Where(pd => pd.SubCategoryID == subCategoryID).ToList(); 
            }

            return PartialView("Partials/_ProductColor", colorModel);
        }
    }
}