using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Report
{
    public class FBRequestReportModel
    {
        public long ID { get; set; }
        public string CustomerID { get; set; }
        public string CompanyCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string CustomerStyleNumber { get; set; }
        public string CompanyShortName { get; set; }
        public string OrderNumber { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? PercentWastage { get; set; }

        public string Remark { get; set; }
        public string Reason { get; set; }
        public string StatusID { get; set; }

        public virtual List<FBRequestReportDetailModel> Details { get; set; }
            = new List<FBRequestReportDetailModel>();
    }
}
