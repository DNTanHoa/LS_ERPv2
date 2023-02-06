using LS_ERP.BusinessLogic.Dtos.OrderDetail;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class OperationDtos
    {

        public string ID { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }      
        public int Index { get; set; }

    }
}
