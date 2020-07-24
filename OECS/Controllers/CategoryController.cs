using OECS.Models;
using OECS.Repository.CategoryRepository;
using OECS.Services.CategoryServices;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController()
        {
            _categoryService = new CategoryService(new CategoryRepository(new oecsEntities()));
        }
        // GET: Category
        public ActionResult Index()
        {
            ViewBag.ModuleTitle = "Category";
            return View();
        }

        public ActionResult CategoryList()
        {
            return PartialView("Partials/_CategoryList", _categoryService.ViewListingCategory());
        }

        public ActionResult SubCategoryList(int? categoryID)
        {
            return PartialView("Partials/_SubCategoryList", _categoryService.ViewListingSubCategory((int)categoryID));
        }
    }
}