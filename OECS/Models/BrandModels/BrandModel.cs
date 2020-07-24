using OECS.Models.CategoryModels;
using System.ComponentModel.DataAnnotations;

namespace OECS.Models.BrandModels
{
    public class BrandModel : SubCategoryModel
    {
        [Key]
        public int BrandID { get; set; }
        [Required]
        public string BrandName { get; set; }
    }
}