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
    
    public partial class Administrator
    {
        public string AdminID { get; set; }
        public string password { get; set; }
        public Nullable<int> RoleID { get; set; }
    
        public virtual Role Role { get; set; }
    }
}
