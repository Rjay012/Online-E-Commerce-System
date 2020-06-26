using OECS.Models.CategoryModels;
using OECS.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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