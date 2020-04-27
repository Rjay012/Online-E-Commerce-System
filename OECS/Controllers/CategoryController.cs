using OECS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Controllers
{
    public class CategoryController : Controller
    {
        oecsEntities dbContext = new oecsEntities();
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show()
        {
            List<Category> categories = dbContext.Category.ToList();
            return PartialView("Partials/_NavbarCategory", categories);
        }
    }
}