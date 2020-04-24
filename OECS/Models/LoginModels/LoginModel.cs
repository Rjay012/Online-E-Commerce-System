using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models.LoginModels
{
    public class LoginModel
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int? RoleID { get; set; }
        public string Role { get; set; }
    }
}