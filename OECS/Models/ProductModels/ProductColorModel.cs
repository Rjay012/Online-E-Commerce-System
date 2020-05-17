using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Models.ProductModels
{
    public class ProductColorModel
    {
        public int ProductColorID { get; set; }
        public int ProductID { get; set; }
        public int ColorID { get; set; }
        public bool IsDisplay { get; set; }
        public bool IsMainDisplay { get; set; }
        public string Path { get; set; }
        public IEnumerable<SelectListItem> ColorList { get; set; }
    }
}