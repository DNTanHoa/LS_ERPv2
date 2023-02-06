using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class AllocTransaction : Audit
    {
        public AllocTransaction(string operation)
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12);
            this.IsRetured = false;
            this.Operation = operation;
        }
        public string ID { get; set; }
        public int DailyTargetDetailID { get; set; }
        public int AllocDailyOuputID { get; set; }
        public string LSStyle { get; set; }
        public string Size { get; set; }
        public string Set { get; set; }
        public decimal AllocQuantity { get; set; }
        public bool IsRetured { get; set; }
        public string Operation { get; set; }
        //for cutting
        public string FabricContrastID { get; set; }
        public string FabricContrastName { get; set; }
        public string Lot { get; set; }
        public int CuttingOutputID { get; set; }
    }
}
