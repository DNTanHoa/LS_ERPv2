using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.Service.Common
{
    public class ConfigSMTP
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public string FromAddress { get; set; }
        public string Password { get; set; }
        public string ToAddress { get; set; }
    }
}
