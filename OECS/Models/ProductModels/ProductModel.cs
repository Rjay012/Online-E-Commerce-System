using OECS.Models.CategoryModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Models.ProductModels
{
    public class ProductModel
    {
        [Key]
        [Required]
        public int ProductID { get; set; }
        [Required]
        [MaxLength(100), MinLength(6)]
        [DisplayName("Product Name:")]
        public string ProductName { get; set; }
        [DisplayName("Date:")]
        public string Date { get; set; }
        [DisplayName("Price:")]
        public decimal Price { get; set; }
        [DisplayName("Description:")]
        public string Description { get; set; }

        public CategoryModel Category { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}