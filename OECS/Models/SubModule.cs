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
    
    public partial class SubModule
    {
        public int SubModuleID { get; set; }
        public string subModule1 { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public string icon { get; set; }
    
        public virtual Module Module { get; set; }
    }
}