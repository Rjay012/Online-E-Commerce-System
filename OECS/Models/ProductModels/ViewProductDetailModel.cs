using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models.ProductModels
{
    public class ViewProductDetailModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public DateTime? Date { get; set; }
        public Decimal? Price { get; set; }
        public string CategoryName { get; set; }
        public int? ColorID { get; set; }
        public int? IconID { get; set; }
        public bool? isMainDisplay { get; set; }
        public bool? display { get; set; }
    }
}