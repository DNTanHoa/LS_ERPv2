using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class ScanResultDto
    {
        public string PONumber { get; set; }
        public string LSStyle { get; set; }
        public string Barcode { get; set; }
        public int? TotalFound { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? TotalBox { get; set; }
        public decimal? Percent { get; set; }

    }
}
