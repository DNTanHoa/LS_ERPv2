using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Plant : Audit
    {
        public Plant()
        {
            this.Code = string.Empty;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string CompanyCode { get; set; }

        public virtual Company Company { get; set; }
        public virtual IList<Storage> Storages { get; set; }
    }
}
