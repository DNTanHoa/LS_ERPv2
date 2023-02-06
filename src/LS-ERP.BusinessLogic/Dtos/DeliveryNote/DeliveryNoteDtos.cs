using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class DeliveryNoteDtos
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string VendorID { get; set; }
        public string VendorName { get; set; }
        public string Status { get; set; }
        public bool IsSend { get; set; }
        public DateTime SendDate { get; set; }
        public bool IsReceived { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Type { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DeliveryNoteDetail> Details { get; set; }
        public List<LSStyleDetailDtos> LSStyleDetails { get; set; }
    }
}
