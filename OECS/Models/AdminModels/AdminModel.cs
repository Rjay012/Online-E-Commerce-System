using OECS.Models.RoleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models.AdminModels
{
    public class AdminModel
    {
        public string AdminID { get; set; }
        public string Password { get; set; }
        public Nullable<int> RoleID { get; set; }

        public RoleModel Role { get; set; }
    }
}