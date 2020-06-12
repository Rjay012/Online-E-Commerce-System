using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace OECS.Models.ProductModels
{
    public class ProductSizeModel
    {
        public int SizeID { get; set; }
        public IEnumerable<SelectListItem> SizeLists
        {
            get
            {
                oecsEntities dbContext = new oecsEntities();
                List<SelectListItem> SizeList = new List<SelectListItem>();
                var size = dbContext.Size.ToList();
                foreach (var item in size)
                {
                    SizeList.Add(new SelectListItem
                    {
                        Value = item.SideID.ToString(),
                        Text = item.size1
                    });
                }
                return SizeList;
            }
        }
    }
}