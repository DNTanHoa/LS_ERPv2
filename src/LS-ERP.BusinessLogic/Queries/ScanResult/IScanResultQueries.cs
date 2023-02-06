using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IScanResultQueries
    {
        public IEnumerable<ScanResultDto> GetSummaryScanResult(string company, DateTime summaryDate);
    }
}
