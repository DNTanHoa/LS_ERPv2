using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class MessageTemplate
    {
        public MessageTemplate()
        {
            Code = string.Empty;
        }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string DisplayName { get; set; }
        public string Template { get; set; }
    }
}
