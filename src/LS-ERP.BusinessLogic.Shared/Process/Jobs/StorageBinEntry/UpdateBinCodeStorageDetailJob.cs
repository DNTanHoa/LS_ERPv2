using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdateBinCodeStorageDetailJob
    {
        private readonly ILogger<UpdateBinCodeStorageDetailJob> logger;
        private readonly SqlServerAppDbContext context;

        public UpdateBinCodeStorageDetailJob(ILogger<UpdateBinCodeStorageDetailJob> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }
        [JobDisplayName("Update Bin Code Storage Detail Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(List<StorageBinEntry> storageBinEntries)
        {
            try
            {
                var updateStorageDetails = new List<StorageDetail>();
                var updateMaterialTrans = new List<MaterialTransaction>();
                var distinctBinEntries = storageBinEntries
                    .Select(x => new {x.CustomerPurchaseOrderNumber, x.StorageCode,
                                      x.CustomerID, x.BinCode }).Distinct().ToList();

                var storageDetails = context.StorageDetail
                    .Where(x => distinctBinEntries.Select(y => y.CustomerID).Contains(x.CustomerID) &&
                           distinctBinEntries.Select(y => y.StorageCode).Contains(x.StorageCode)).ToList();

                distinctBinEntries.ForEach(e =>
                {
                    var selectedStorageDetails = storageDetails
                        .Where(x =>(x.PurchaseOrderNumber ?? string.Empty).Trim().ToUpper() == e.CustomerPurchaseOrderNumber.Trim().ToUpper()).ToList();
                    selectedStorageDetails.ForEach(y =>
                    {
                        y.StorageBinCode = e.BinCode;
                        updateStorageDetails.Add(y);

                        var selectedMaterialTrans = context.MaterialTransaction
                            .Where(t => t.StorageDetailID == y.ID) .ToList();
                        selectedMaterialTrans.ForEach(t =>
                        {
                            t.StorageBinCode = e.BinCode;
                            updateMaterialTrans.Add(t);
                        });
                    });
                });

                context.UpdateRange(updateStorageDetails);
                context.UpdateRange(updateMaterialTrans);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Update bin code storage detail has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Update bin code storage detail has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
