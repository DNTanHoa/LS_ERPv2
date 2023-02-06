using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class SalesContract : Audit
    {
        public SalesContract()
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12);
            Number = "CTR" + DateTime.Now.Month + "/" + DateTime.Now.Year.ToString().Substring(2, 2) + "-";
        }
        public string ID { get; set; }
        public string Number { get; set; }
        public string CustomerID { get; set; }
        public string FileName { get; set; }
        public string SaveFilePath { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual IList<SalesContractDetail> ContractDetails { get; set; }

    }
}
