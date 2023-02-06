using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class StorageDetailRepository : IStorageDetailRepository
    {
        private readonly AppDbContext context;

        public StorageDetailRepository(AppDbContext context)
        {
            this.context = context;
        }

        public StorageDetail Add(StorageDetail storageDetail)
        {
            return context.StorageDetail.Add(storageDetail).Entity;
        }

        public void Delete(StorageDetail storageDetail)
        {
            context.StorageDetail.Remove(storageDetail);
        }

        public StorageDetail GetStorageDetail(long ID)
        {
            return context.StorageDetail.FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<StorageDetail> GetStorageDetails()
        {
            return context.StorageDetail;
        }

        public IQueryable<StorageDetail> GetStorageDetails(string storageCode)
        {
            return context.StorageDetail
                .Include(x => x.ReservationEntries)
                .Include(x => x.MaterialTransactions)
                .Where(x => x.StorageCode == storageCode);
        }

        public IQueryable<StorageDetail> GetStorageDetails(string storageCode,
            string purchaseOrderNumber)
        {
            return context.StorageDetail
                .Include(x => x.ReservationEntries)
                .Include(x => x.MaterialTransactions)
                .Where(x => x.PurchaseOrderNumber == purchaseOrderNumber);
        }

        public IQueryable<StorageDetail> GetStorageDetails(List<long> storageDetailIDs)
        {
            return context.StorageDetail
                .Include(x => x.ReservationEntries)
                .Include(x => x.MaterialTransactions)
                .Where(x => storageDetailIDs.Contains(x.ID));
        }

        public IQueryable<StorageDetail> GetFabricStorageDetails(string fabricPurchaseOrderNumber)
        {
            return context.StorageDetail
                .Include(x => x.ReservationEntries)
                .Include(x => x.MaterialTransactions)
                .Where(x => x.FabricPurchaseOrderNumber == fabricPurchaseOrderNumber);
        }

        public IQueryable<StorageDetail> GetStorageDetailsForCustomer(string storageCode, string customerID)
        {
            return context.StorageDetail
                .Include(x => x.ReservationEntries)
                .Include(x => x.MaterialTransactions)
                .Where(x => x.CustomerID == customerID);
        }

        public void Update(StorageDetail storageDetail)
        {
            context.Entry(storageDetail).State = EntityState.Modified;
        }

        public IQueryable<StorageDetail> GetOnlyStorageDetails(List<long> storageDetailIDs)
        {
            return context.StorageDetail
                .Where(x => storageDetailIDs.Contains(x.ID));
        }

        public IQueryable<StorageDetail> GetOnlyStorageDetailsForCustomer(string storageCode, string customerID)
        {
            return context.StorageDetail
                .Where(x => x.CustomerID == customerID && x.StorageCode == storageCode);
        }
    }
}
