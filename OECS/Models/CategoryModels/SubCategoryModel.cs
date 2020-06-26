using System.ComponentModel.DataAnnotations;

namespace OECS.Models.CategoryModels
{
    public class SubCategoryModel : CategoryModel
    {
        [Key]
        public int? SubCategoryID { get; set; }
        [Required]
        public string SubCategory { get; set; }
    }
}