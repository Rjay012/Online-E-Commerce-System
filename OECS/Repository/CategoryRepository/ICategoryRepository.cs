using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OECS.Repository.CategoryRepository
{
    public interface ICategoryRepository
    {
        IEnumerable<SelectListItem> ListCategory();
    }
}
