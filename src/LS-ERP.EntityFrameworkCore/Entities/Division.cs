using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Division : Audit
    {
        public Division()
        {
            ID = string.Empty;
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
