using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class WorkCenterType
    {
        public WorkCenterType()
        {
            this.ID = string.Empty;
        }
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
