using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Events
{
    public class OrderDetailBulkUpdatedEvent : INotification
    {
        public List<OrderDetail> UpdatedOrderDetails { get; set; }
        public string UserName { get; set; }
    }
}
