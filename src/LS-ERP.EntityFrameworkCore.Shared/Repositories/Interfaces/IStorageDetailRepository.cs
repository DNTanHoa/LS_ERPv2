using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IStorageDetailRepository
    {
        IQueryable<StorageDetail> GetStorageDetails();
        IQueryable<StorageDetail> GetStorageDetails(string storageCode);
        IQueryable<StorageDetail> GetFabricStorageDetails(string fabricPurchaseOrderNumber);
        IQueryable<StorageDetail> GetStorageDetails(List<long> storageDetailIDs);
        IQueryable<StorageDetail> GetOnlyStorageDetails(List<long> storageDetailIDs);
        IQueryable<StorageDetail> GetStorageDetails(string storageCode, string purchaseOrderNumber);
        IQueryable<StorageDetail> GetStorageDetailsForCustomer(string storageCode,
            string customerID);
        IQueryable<StorageDetail> GetOnlyStorageDetailsForCustomer(string storageCode,
            string customerID);
        StorageDetail GetStorageDetail(long ID);
        StorageDetail Add(StorageDetail storageDetail);
        void Update(StorageDetail storageDetail);
        void Delete(StorageDetail storageDetail);
    }
}
