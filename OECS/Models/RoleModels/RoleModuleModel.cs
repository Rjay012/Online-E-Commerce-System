using OECS.Models.ModuleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models.RoleModels
{
    public class RoleModuleModel
    {
        public int RoleModID { get; set; }
        public Nullable<int> ModuleID { get; set; }
        public Nullable<int> RoleID { get; set; }

        public ModuleModel Module { get; set; }
        public RoleModel Role { get; set; }
        public ICollection<SubModuleModel> SubModule { get; set; }
    }
}