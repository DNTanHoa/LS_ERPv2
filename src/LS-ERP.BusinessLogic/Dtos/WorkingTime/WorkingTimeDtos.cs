using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class WorkingTimeDtos
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SortIndex { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
    }
}
