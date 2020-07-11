using OECS.Models.CategoryModels;
using System.ComponentModel.DataAnnotations;

namespace OECS.Models.ProductModels.ProductDetailModels.ColorModels
{
    public class ColorModel : SubCategoryModel
    {
        [Key]
        public int ColorID { get; set; }
        public string Color { get; set; }
    }
}