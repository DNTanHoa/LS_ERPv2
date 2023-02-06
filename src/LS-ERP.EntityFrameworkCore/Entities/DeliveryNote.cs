using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class DeliveryNote : Audit
    {
        public DeliveryNote()
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLMNOPQ", 12); 
        }
        public string ID { get; set; }
        public string Code { get; set; }
        public string CompanyID { get; set; }   
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string Status { get; set; }
        public bool IsSend { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsReceived { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Type { get; set; }
        public string Remark { get; set; }
      
    }
}
