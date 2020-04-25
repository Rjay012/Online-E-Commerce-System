using OECS.Models.AdminModels;
using OECS.Models.ModuleModels;
using OECS.Models.RoleModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models
{
    public class ViewModuleModel
    {
        public Module Module { get; set; }
        public SubModule SubModule { get; set; }
        public Role Role { get; set; }
        public RoleModule RoleModuleModel { get; set; }
    }
}