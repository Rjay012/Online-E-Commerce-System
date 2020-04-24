using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OECS.Models.LoginModels
{
    public class RegisterModel
    {
        public int? UserID { get; set; }
        public string Firstname { get; set; }
        public string Middlename { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string RetypePassword { get; set; }
    }
}