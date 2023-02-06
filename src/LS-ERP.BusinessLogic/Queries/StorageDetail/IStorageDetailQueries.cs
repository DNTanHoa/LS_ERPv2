using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IStorageDetailQueries
    {
        public List<StorageDetail> GetStorageDetails();
        public List<StorageDetail> GetAllStorageDetails(string storageCode);
        public List<StorageDetail> GetAllStorageDetails(string itemID, string customerID);
        public List<StorageDetailDto> GetSummaryStorageDetailReports(string customerID, string storageCode,
            DateTime? fromDate, DateTime? toDate, string productionMethodCode, decimal onHandQuantity);
    }
}
