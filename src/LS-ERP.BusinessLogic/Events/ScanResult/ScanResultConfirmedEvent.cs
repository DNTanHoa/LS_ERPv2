using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Events
{
    public class ScanResultConfirmedEvent : INotification
    {
        public string UserName { get;set; }
        public int PeriodID { get; set; }
    }
}
