using OECS.Models.ProductModels;
using OECS.Models.SupplierModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OECS.Models.SupplyModels
{
    public class SupplyModel
    {
        [Key]
        public int SupplyID { get; set; }
        [ForeignKey("ProductModel")]
        public int? ProductID { get; set; }
        [ForeignKey("SupplierModel")]
        public int? SupplierID { get; set; }
        public int? quantity { get; set; }

    }
}