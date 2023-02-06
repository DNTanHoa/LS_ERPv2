using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Item : Audit
    {
        public string Code { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string Season { get; set; }
        public string CustomerID { get; set; }
        public string Specify { get; set; }
        public string MaterialTypeCode { get; set; }    
        public virtual MaterialType MaterialType { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
