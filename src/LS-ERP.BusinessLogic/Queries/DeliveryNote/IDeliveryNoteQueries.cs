using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IDeliveryNoteQueries
    {
        public IEnumerable<DeliveryNoteDtos> GetByID(string ID);
        public IEnumerable<DeliveryNoteDtos> GetByCompany(string companyID,DateTime fromDate,DateTime toDate);
        public IEnumerable<DeliveryNoteDtos> GetForScanQrcode(string companyID, string type);
        public IEnumerable<DeliveryNoteDetailDtos> GetReport(string companyID, DateTime fromDate,DateTime toDate,string keyWord);
        public IEnumerable<LSStyleDetailReportDtos> GetReceiveReport(string companyID, DateTime fromDate, DateTime toDate, string keyWord);
    }
}
