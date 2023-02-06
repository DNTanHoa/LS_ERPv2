using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Events
{
    public class ItemStyleBulkedEvent : INotification
    {
        public string UserName { get; set; } = string.Empty;
        public List<string> ItemStyleNumbers { get; set; } = new List<string>();
    }
}
