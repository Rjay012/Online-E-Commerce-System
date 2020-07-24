using OECS.Models.RoleModels;
using System;
using System.Collections.Generic;

namespace OECS.Models.ModuleModels
{
    public class ModuleModel
    {
        public int ModuleID { get; set; }
        public string Module { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public Nullable<bool> HasChild { get; set; }
        public Nullable<bool> HasArrow { get; set; }
        public ICollection<RoleModuleModel> RoleModule { get; set; }
    }
}