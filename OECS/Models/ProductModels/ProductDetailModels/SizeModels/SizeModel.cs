using OECS.Models.CategoryModels;
using System.ComponentModel.DataAnnotations;

namespace OECS.Models.ProductModels.ProductDetailModels.SizeModels
{
    public class SizeModel : SubCategoryModel
    {
        [Key]
        public int SideID { get; set; }
        public string Size { get; set; }
    }
}