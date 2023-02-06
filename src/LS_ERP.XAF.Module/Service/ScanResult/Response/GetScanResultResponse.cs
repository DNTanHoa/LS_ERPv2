using DevExpress.Utils.DPI;
using DevExpress.Xpo.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.XAF.Module.Service.Response
{
    public class GetScanResultResponse : CommonRespone
    {
        public List<GetScanResultResponseData> Data { get; set; } = new List<GetScanResultResponseData>();
    }

    public class GetScanResultResponseData
    {
        public string PONumber { get; set; }
        public string LSStyle { get; set; }
        public string BarCode { get; set; }
        public decimal? TotalFound { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? TotalBox { get; set; }
        public decimal? Percent { get; set; }
    }
}
