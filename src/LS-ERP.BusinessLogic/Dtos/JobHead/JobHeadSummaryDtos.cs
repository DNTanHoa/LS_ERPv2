using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class JobHeadSummaryDtos
    {
        public string Number { get; set; }
        public string GarmentColorCode { get; set; }
        public string GarmentSize { get; set; }
        public string CustomerStyle { get; set; }
        public string LSStyle { get; set; }
        public string Config { get; set; }
        public long? PartRevisionID { get; set; }
        public decimal? ProductionQuantity { get; set; }
        public DateTime? RequestDueDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? Confirmed { get; set; }
    }
}
