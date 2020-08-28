using System;
using System.Collections.Generic;

namespace OECS.Models.ProductModels.ProductDetailModels
{
    public class ViewProductDetailModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public DateTime? Date { get; set; }
        public Decimal? Price { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public int? SizeID { get; set; }
        public int? ColorID { get; set; }  //removed nullable
        public int? IconID { get; set; }
        public bool? isMainDisplay { get; set; }
        public bool? display { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}