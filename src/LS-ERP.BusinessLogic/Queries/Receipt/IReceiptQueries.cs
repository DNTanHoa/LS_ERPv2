using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IReceiptQueries
    {
        public List<ReceiptDto> GetReceiptSummary(string numbers, string storageCode,
            DateTime fromDate, DateTime toDate);
    }
}
