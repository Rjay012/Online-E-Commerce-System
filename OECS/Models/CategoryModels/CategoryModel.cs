using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OECS.Models.CategoryModels
{
    public class CategoryModel
    {
        [Key]
        [Required]
        public int CategoryID { get; set; }
        [Required]
        [DisplayName("Category:")]
        public string Category { get; set; }
    }
}