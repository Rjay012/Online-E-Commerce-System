//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OECS.Models
{
    using System;
    
    public partial class CustomerOrder_Result
    {
        public string OrderNumber { get; set; }
        public string productName { get; set; }
        public string BrandName { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> Price { get; set; }
        public int ProductID { get; set; }
        public int ColorID { get; set; }
    }
}