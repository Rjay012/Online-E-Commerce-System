using OECS.Models.ProductModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OECS.Models.StockModels
{
    public class StockModel
    {
        [Key]
        public int StockID { get; set; }
        public int? quantity { get; set; }
        [ForeignKey("ProductModel")]
        public int? ProductID { get; set; }

    }
}