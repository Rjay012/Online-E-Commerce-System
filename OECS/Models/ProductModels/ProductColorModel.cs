using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Models.ProductModels
{
    public class ProductColorModel
    {
        public int ProductColorID { get; set; }
        public int ProductID { get; set; }
        [Required]
        public int ColorID { get; set; }
        public int IsDisplayPosition { get; set; }
        [Required]
        public bool? IsDisplay { get; set; }
        public bool ToDisplay { get; set; }
        public string Path { get; set; }
        public string IconPath { get; set; }
        [Required]
        public HttpPostedFileBase IconFile { get; set; }
        [Required]
        public HttpPostedFileBase[] Files {get; set;}

        public IEnumerable<SelectListItem> ColorList { get; set; }
    }
}