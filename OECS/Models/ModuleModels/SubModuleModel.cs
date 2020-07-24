using OECS.Models.RoleModels;
using System;

namespace OECS.Models.ModuleModels
{
    public class SubModuleModel
    {
        public int SubModuleID { get; set; }
        public string SubModule { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public Nullable<int> RolModID { get; set; }
        public string Icon { get; set; }
        public Nullable<bool> HasArrow { get; set; }
        public Nullable<bool> HasParent { get; set; }

        public RoleModuleModel RoleModule { get; set; }
    }
}