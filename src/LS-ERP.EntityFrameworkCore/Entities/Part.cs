using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class Part : Audit
    {
        public Part()
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12);
        }
        public string ID { get; set; }
        public string Number { get; set; } // style no, customer style
        public string Description { get; set; }
        public string Image { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentColorName { get; set; }
        public string Season { get; set; }
        public int? LastSequenceNumber { get; set; }
        public string CustomerID { get; set; }
        public string Character { get; set; }
        public string ContractNo { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
