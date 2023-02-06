using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Entities
{
    public class PackingSheetName : Audit
    {
        public long ID { get; set; }
        public string SheetName { get; set; }
        public int? SheetSortIndex { get; set; }    

    }
}
