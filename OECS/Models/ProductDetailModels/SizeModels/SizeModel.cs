using OECS.Models.CategoryModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OECS.Models.ProductDetailModels.SizeModels
{
    public class SizeModel : SubCategoryModel
    {
        [Key]
        public int SideID { get; set; }
        public string Size { get; set; }
    }
}