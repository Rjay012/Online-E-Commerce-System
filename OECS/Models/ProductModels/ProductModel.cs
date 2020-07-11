using OECS.Models.BrandModels;
using OECS.Models.CategoryModels;
using OECS.Models.StockModels;
using OECS.Models.SupplyModels;
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
        public int ProductID { get; set; }
        [Required]
        [MaxLength(100), MinLength(6)]
        [DisplayName("Product Name:")]
        public string ProductName { get; set; }
        [Required]
        [DisplayName("Date:")]
        public string Date { get; set; }
        [Required]
        [DisplayName("Price:")]
        public decimal? Price { get; set; }
        [DisplayName("Description:")]
        public string Description { get; set; }
        public bool? Display { get; set; }
        [Required]
        [ForeignKey("SubCategoryModel")]
        [DisplayName("Category:")]
        public int? SubCategoryID { get; set; }
        [ForeignKey("BrandModel")]
        [DisplayName("Brand:")]
        public int? BrandID { get; set; }

        public IEnumerable<SelectListItem> CategorySelectList { get; set; }

        public IEnumerable<SelectListItem> BrandSelectList { get; set; }
    }
}