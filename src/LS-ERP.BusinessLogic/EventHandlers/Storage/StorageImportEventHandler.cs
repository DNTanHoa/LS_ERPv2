using LS_ERP.BusinessLogic.Events;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.EventHandlers
{
    public class StorageImportEventHandler : INotificationHandler<StorageImportEvent>
    {
        private readonly ILogger<StorageImportEventHandler> logger;
        private readonly SqlServerAppDbContext context;

        public StorageImportEventHandler(ILogger<StorageImportEventHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public Task Handle(StorageImportEvent notification, 
            CancellationToken cancellationToken)
        {
            try
            {
                var materialTransactions = new List<MaterialTransaction>();

                var storageImport = context.StorageImport
                    .Include(x => x.Details)
                    .FirstOrDefault(x => x.ID == notification.StorageImportID);

                var storageDetails = context.StorageDetail
                    .Where(x => x.StorageCode == storageImport.StorageCode &&
                                x.CustomerID == storageImport.CustomerID).ToList();

                if(storageImport != null)
                {
                    if(storageImport.Details != null &&
                       storageImport.Details.Any())
                    {
                        foreach(var detail in storageImport.Details)
                        {
                            var materialTransaction = new MaterialTransaction()
                            {
                                ItemCode = detail.ItemCode,
                                ItemID = detail.ItemID,
                                ItemName = detail.ItemName,
                                ItemColorCode = detail.ItemColorCode,
                                ItemColorName = detail.ItemColorName,
                                Position = detail.ItemID,
                                Specify = detail.ItemID,
                                GarmentColorCode = detail.GarmentColorCode,
                                GarmentColorName = detail.GarmentColorName,
                                GarmentSize = detail.GarmentSize,
                                CustomerStyle = detail.CustomerStyle,
                                LSStyle = detail.LSStyle,
                                UnitID = detail.UnitID,
                                Quantity = detail.Quantity,
                                Roll = detail.Roll,
                                StorageImportDetailID = detail.ID,
                                StorageCode = storageImport.StorageCode,
                            };

                            materialTransactions.Add(materialTransaction);
                        }
                    }
                }

                foreach(var storageDetail in storageDetails)
                {
                    var transactions = materialTransactions
                        .Where(x => x.ItemID == storageDetail.ItemID &&
                                    x.ItemName == storageDetail.ItemName &&
                                    x.ItemColorCode == storageDetail.ItemColorCode &&
                                    x.ItemColorName == storageDetail.ItemColorName &&
                                    x.Specify == storageDetail.Specify &&
                                    x.Position == storageDetail.Position &&
                                    x.GarmentColorCode == storageDetail.GarmentColorCode &&
                                    x.GarmentColorName == storageDetail.GarmentColorName &&
                                    x.GarmentSize == storageDetail.GarmentSize);

                    foreach(var transaction in transactions)
                    {
                        transaction.StorageCode = storageDetail.StorageCode;
                        transaction.StorageDetailID = storageDetail.ID;
                    }

                    storageDetail.OnHandQuantity += transactions.Sum(x => x.Quantity);

                }

                context.StorageDetail.UpdateRange(storageDetails);
                context.MaterialTransaction.AddRange(materialTransactions);

                context.SaveChanges();
            }
            catch(Exception ex)
            {
                logger.LogError("{@time} - Storage import event handler has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Storage import event handler has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
