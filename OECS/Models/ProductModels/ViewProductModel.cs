using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models.ProductModels
{
    public class ViewProductModel
    {
        public Product Product { get; set; }
        public ProductImage ProductImage { get; set; }
        public Color Color { get; set; }
        public ProductColor ProductColor { get; set; }
        public Category Category { get; set; }
    }
}