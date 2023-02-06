using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class BoxGroup : Audit
    {
        public BoxGroup()
        {
            Oid = Guid.NewGuid().ToString();
        }
        public string Oid { get; set; }
        public string BarcodeRange { get; set; }
        public string StartBarcode { get; set; }
        public string EndBarcode { get; set; }
        public string PONum { get; set; }
        public DateTime? Date { get; set; }
        public string FileName { get; set; }
        public int? OptimisticLockField { get; set; }    
        public int? GCRecord { get; set; }  
        public string SheetName { get; set; }
        public bool? IsPulled { get; set; }
        public string PackingListCode { get; set; } 
        public virtual List<BoxDetail> BoxDetails { get; set; } = new List<BoxDetail>();
        public virtual List<BoxModel> BoxModels { get; set; } = new List<BoxModel>();

        public string CustomerID { get; set; }
    }
}
