using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class OperationDetail : Audit
    {
        public OperationDetail()
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12); 
        }
        public string ID { get; set; }       
        public string MergeBlockLSStyle { get; set; }   
        public string Set { get; set; }
        public int FabricContrastID { get; set; }
        public bool IsPercentPrint { get; set; }
        public string FabricContrastName { get; set; }
        public string OperationID { get; set; }
        public string OperationName { get; set; }
    }
}
