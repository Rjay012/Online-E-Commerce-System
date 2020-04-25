using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OECS.Models.LoginModels
{
    public class LoginModel
    {
        public string UserID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int? RoleID { get; set; }
        public string Role { get; set; }

        public IEnumerable<SelectListItem> RoleList { get; set; }
    }
}