using OECS.Models.RoleModels;
using OECS.Models.SupplyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models.SupplierModels
{
    public class SupplierModel
    {
        public int SupplierID { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public Nullable<int> RoleID { get; set; }

        public RoleModel Role { get; set; }
        public List<SupplyModel> Supplies { get; set; }
    }
}