using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Events
{
    public class ReceiptCreatedEvent : INotification
    {
        public string ReceiptNumber { get; set; }
        public string StorageCode { get; set; }
        public string UserName { get; set; }
    }
}
