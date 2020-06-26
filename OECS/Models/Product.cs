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
    using System.Collections.Generic;
    
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.ProductDetail = new HashSet<ProductDetail>();
            this.Stock = new HashSet<Stock>();
            this.Supply = new HashSet<Supply>();
        }
    
        public int ProductID { get; set; }
        public string productName { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<decimal> price { get; set; }
        public string description { get; set; }
        public Nullable<bool> display { get; set; }
        public Nullable<int> BrandID { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductDetail> ProductDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Stock> Stock { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Supply> Supply { get; set; }
    }
}
