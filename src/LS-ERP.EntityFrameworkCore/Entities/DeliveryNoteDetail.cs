using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ultils.Helpers;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class DeliveryNoteDetail : Audit
    {
        public DeliveryNoteDetail()
        {
            ID = Nanoid.Nanoid.Generate("123456789ABCDEFGHIJKLM", 12); 
        }
        public string ID { get; set; }
        public string CompanyID { get; set; }
        public string DeliveryNoteID { get; set; }
        public string CuttingCardID { get; set; }
        public string MergeBlockLSStyle { get; set; }
        public string MergeLSStyle { get; set; }
        public string FabricContrastName { get; set; }
        public string FabricContrastColor { get; set; }
        public string Set { get; set; }
        public string Size { get; set; }
        public string WorkCenterName { get; set; }
        public int TableNO { get; set; }
        public int TotalPackage { get; set; }
        public DateTime ProduceDate { get; set; }
        public decimal AllocQuantity { get; set; }
        public string Status { get; set; }
        public bool IsSend { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsReceived { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string CardType { get; set; }

    }
}
