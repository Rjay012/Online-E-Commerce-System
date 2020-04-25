using OECS.Models.AdminModels;
using OECS.Models.CustomerModels;
using OECS.Models.SupplierModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models.RoleModels
{
    public class RoleModel
    {
        public RoleModel()
        {
            Administrator = new List<AdminModel>();
            Customer = new List<CustomerModel>();
            Supplier = new List<SupplierModel>();
            RoleModule = new List<RoleModuleModel>();
        }

        public int RoleID { get; set; }
        public string Role { get; set; }

        public List<AdminModel> Administrator { get; set; }
        public List<CustomerModel> Customer { get; set; }
        public List<SupplierModel> Supplier { get; set; }
        public List<RoleModuleModel> RoleModule { get; set; }
    }
}