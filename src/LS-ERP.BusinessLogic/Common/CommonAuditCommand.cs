using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Common
{
    public class CommonAuditCommand
    {
        public string Username { get; set; }

        public CommonAuditCommand SetUser(string username)
        {
            this.Username = username;
            return this;
        }

        public string GetUser() => this.Username;
    }
}
