using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OECS.Models.CategoryModels
{
    public class CategoryModel
    {
        [Key]
        public int? CategoryID { get; set; }
        [DisplayName("Category:")]
        public string Category { get; set; }
    }
}