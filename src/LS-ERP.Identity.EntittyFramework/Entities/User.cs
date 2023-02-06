using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Identity.EntittyFramework.Entities
{
    public class User
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string HashKey { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
