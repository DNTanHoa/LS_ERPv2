using AutoMapper;
using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class CreateIssuedFabricJob
    {
        private readonly ILogger<CreateIssuedFabricJob> logger;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IFabricRequestDetailRepository fabricRequestDetailRepository;
        private readonly IFabricRequestRepository fabricRequestRepository;
        private readonly IFabricPurchaseOrderRepository fabricPurchaseOrderRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;

        public CreateIssuedFabricJob(ILogger<CreateIssuedFabricJob> logger,
            IMapper mapper,
            SqlServerAppDbContext context,
            IStorageDetailRepository storageDetailRepository,
            IFabricRequestDetailRepository fabricRequestDetailRepository,
            IFabricRequestRepository fabricRequestRepository,
            IFabricPurchaseOrderRepository fabricPurchaseOrderRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.context = context;
            this.storageDetailRepository = storageDetailRepository;
            this.fabricRequestDetailRepository = fabricRequestDetailRepository;
            this.fabricRequestRepository = fabricRequestRepository;
            this.fabricPurchaseOrderRepository = fabricPurchaseOrderRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
        }

        [JobDisplayName("Create Issued Fabric For Import Output Storage Detail Job")]
        [AutomaticRetry(Attempts = 1)]
        public Task Execute(int StorageImportID)
        {
            var storageImport = context.StorageImport
                    .Include(x => x.Details)
                    .FirstOrDefault(x => x.ID == StorageImportID);

            //using var transaction = context.Database.BeginTransaction();
            //transaction.CreateSavepoint("SaveIssuedFabric");
            Dictionary<string, Issued> dicIssued = new Dictionary<string, Issued>();

            foreach (var item in storageImport.Details)
            {
                if (item.StorageDetailID > 0)
                {
                    var key = item.TransactionDate?.ToString().Replace(" ", "") + item.OutputOrder?.Replace(" ", "").Trim().ToUpper();

                    if (dicIssued.TryGetValue(key, out Issued rsIssued))
                    {
                        IssuedGroupLine issuedGroupLine = new IssuedGroupLine();
                        issuedGroupLine.ItemID = item.ItemID;
                        issuedGroupLine.ItemCode = item.ItemCode;
                        issuedGroupLine.ItemName = item.ItemName.Replace("\n", "");
                        issuedGroupLine.ItemColorCode = item.ItemColorCode.Replace("\n", "");
                        issuedGroupLine.ItemColorName = item.ItemColorName.Replace("\n", "");
                        issuedGroupLine.Position = item.Position;
                        issuedGroupLine.Specify = item.Specify;
                        issuedGroupLine.Season = item.Season;
                        issuedGroupLine.UnitID = item.UnitID;
                        issuedGroupLine.CustomerStyle = item.CustomerStyle;
                        issuedGroupLine.GarmentColorCode = item.GarmentColorCode;
                        issuedGroupLine.GarmentColorName = item.GarmentColorName;
                        issuedGroupLine.IssuedQuantity = item.Quantity;
                        issuedGroupLine.Roll = item.Roll;
                        issuedGroupLine.StorageDetailID = item.StorageDetailID;
                        issuedGroupLine.LotNumber = item.LotNumber;
                        issuedGroupLine.DyeLotNumber = item.DyeLotNumber;
                        issuedGroupLine.FabricPurchaseOrderNumber = item.FabricPurchaseOrderNumber;

                        IssuedLine issuedLine = new IssuedLine();
                        issuedLine.ItemID = item.ItemID;
                        issuedLine.ItemCode = item.ItemCode;
                        issuedLine.ItemName = item.ItemName.Replace("\n", "");
                        issuedLine.ItemColorCode = item.ItemColorCode.Replace("\n", "");
                        issuedLine.ItemColorName = item.ItemColorName.Replace("\n", "");
                        issuedLine.Position = item.Position;
                        issuedLine.Specify = item.Specify;
                        issuedLine.Season = item.Season;
                        issuedLine.UnitID = item.UnitID;
                        issuedLine.CustomerStyle = item.CustomerStyle;
                        issuedLine.GarmentColorCode = item.GarmentColorCode;
                        issuedLine.GarmentColorName = item.GarmentColorName;
                        issuedLine.IssuedQuantity = item.Quantity;
                        issuedLine.Roll = item.Roll;
                        issuedLine.StorageDetailID = item.StorageDetailID;
                        issuedLine.LotNumber = item.LotNumber;
                        issuedLine.DyeLotNumber = item.DyeLotNumber;
                        issuedLine.FabricPurchaseOrderNumber = item.FabricPurchaseOrderNumber;

                        issuedGroupLine.IssuedLines.Add(issuedLine);
                        rsIssued.IssuedGroupLines.Add(issuedGroupLine);
                        dicIssued[key] = rsIssued;
                    }
                    else
                    {
                        Issued newIssued = new Issued();
                        newIssued.DocumentReferenceNumber = item.DocumentNumber;
                        newIssued.InvoiceNumber = item.InvoiceNumber;
                        newIssued.Description = item.OutputOrder;
                        newIssued.Remark = item.Remark;
                        newIssued.IssuedDate = item.TransactionDate;
                        newIssued.IssuedBy = "Admin";
                        newIssued.ReceivedBy = "Admin";
                        newIssued.SetCreateAudit("Admin");
                        newIssued.CustomerID = storageImport.CustomerID;
                        newIssued.StorageCode = storageImport.StorageCode;
                        newIssued.InvoiceNumberNoTotal = item.InvoiceNumberNoTotal;
                        newIssued.IssuedTypeId = item.ProductionMethodCode;

                        IssuedGroupLine issuedGroupLine = new IssuedGroupLine();
                        issuedGroupLine.ItemID = item.ItemID;
                        issuedGroupLine.ItemCode = item.ItemCode;
                        issuedGroupLine.ItemName = item.ItemName.Replace("\n", "");
                        issuedGroupLine.ItemColorCode = item.ItemColorCode.Replace("\n", "");
                        issuedGroupLine.ItemColorName = item.ItemColorName.Replace("\n", "");
                        issuedGroupLine.Position = item.Position;
                        issuedGroupLine.Specify = item.Specify;
                        issuedGroupLine.Season = item.Season;
                        issuedGroupLine.UnitID = item.UnitID;
                        issuedGroupLine.CustomerStyle = item.CustomerStyle;
                        issuedGroupLine.GarmentColorCode = item.GarmentColorCode;
                        issuedGroupLine.GarmentColorName = item.GarmentColorName;
                        issuedGroupLine.IssuedQuantity = item.Quantity;
                        issuedGroupLine.Roll = item.Roll;
                        issuedGroupLine.StorageDetailID = item.StorageDetailID;
                        issuedGroupLine.LotNumber = item.LotNumber;
                        issuedGroupLine.DyeLotNumber = item.DyeLotNumber;
                        issuedGroupLine.FabricPurchaseOrderNumber = item.FabricPurchaseOrderNumber;

                        IssuedLine issuedLine = new IssuedLine();
                        issuedLine.ItemID = item.ItemID;
                        issuedLine.ItemCode = item.ItemCode;
                        issuedLine.ItemName = item.ItemName.Replace("\n", "");
                        issuedLine.ItemColorCode = item.ItemColorCode.Replace("\n", "");
                        issuedLine.ItemColorName = item.ItemColorName.Replace("\n", "");
                        issuedLine.Position = item.Position;
                        issuedLine.Specify = item.Specify;
                        issuedLine.Season = item.Season;
                        issuedLine.UnitID = item.UnitID;
                        issuedLine.CustomerStyle = item.CustomerStyle;
                        issuedLine.GarmentColorCode = item.GarmentColorCode;
                        issuedLine.GarmentColorName = item.GarmentColorName;
                        issuedLine.IssuedQuantity = item.Quantity;
                        issuedLine.Roll = item.Roll;
                        issuedLine.StorageDetailID = item.StorageDetailID;
                        issuedLine.LotNumber = item.LotNumber;
                        issuedLine.DyeLotNumber = item.DyeLotNumber;
                        issuedLine.FabricPurchaseOrderNumber = item.FabricPurchaseOrderNumber;

                        issuedGroupLine.IssuedLines.Add(issuedLine);
                        newIssued.IssuedGroupLines.Add(issuedGroupLine);

                        dicIssued[key] = newIssued;
                    }
                }
            }

            var number = entitySequenceNumberRepository.GetNextNumberByCode("Issued",
                out EntitySequenceNumber sequenceNumber);

            var dicIssuedNumber = new Dictionary<string, string>();
            dicIssuedNumber[number] = number;

            foreach (var issued in dicIssued)
            {
                var nextNumber = sequenceNumber.Prefix + sequenceNumber.Subfix
                    + sequenceNumber.LastNumber.ToString().PadLeft(12, '0');

                if (dicIssuedNumber.ContainsKey(nextNumber))
                {
                    sequenceNumber.LastNumber += 1;

                    nextNumber = sequenceNumber.Prefix + sequenceNumber.Subfix
                    + sequenceNumber.LastNumber.ToString().PadLeft(12, '0');
                }

                issued.Value.Number = nextNumber;
                dicIssuedNumber[nextNumber] = nextNumber;

                sequenceNumber.LastNumber += 1;

                ///Map issued line
                issued.Value.IssuedGroupLines
                    .SelectMany(x => x.IssuedLines).ToList().ForEach(x =>
                    {
                        x.IssuedNumber = issued.Value.Number;
                    });

                ///Create material transactions
                var transactions = MaterialTransactionProcess
                    .CreateTransactionsFB(issued.Value, "Admin");

                ///Canculate and update storage detail
                var currentStorageDetails = storageDetailRepository
                    .GetStorageDetails(issued.Value.IssuedGroupLines.Select(x => x.StorageDetailID).ToList());

                var updateStorageDetailResult = StorageDetailProcess.UpdateFabricStorageFromImportOutputIssued(currentStorageDetails.ToList(),
                    transactions, issued.Value.StorageCode, "Admin", out string errorMesssage);

                ///Update Fabric purchase order information data
                if (!string.IsNullOrEmpty(issued.Value.IssuedTypeId) && issued.Value.IssuedTypeId.Equals("CMT"))
                {
                    var fabricPurchaseOrders = fabricPurchaseOrderRepository
                   .GetFabricPurchaseOrders(currentStorageDetails.Select(x => x.FabricPurchaseOrderNumber).ToList());

                    ImportFabricPurchaseOrderProcessor.CalculateIssuedQuantity(fabricPurchaseOrders.ToList(), currentStorageDetails.ToList(),
                        issued.Value?.IssuedGroupLines.ToList(),
                        out string errorMessage);

                    context.FabricPurchaseOrder.UpdateRange(fabricPurchaseOrders);
                }

                //if (request.FabricRequestID != null && request.FabricRequestID != 0)
                //{
                //    var fabricRequestDetails = fabricRequestDetailRepository.GetFabricRequestDetails((long)request.FabricRequestID)?.ToList();
                //    var fabricRequest = fabricRequestRepository.GetFabricRequest(request.FabricRequestID);

                //    FabricRequestProcessor.UpdateFabricRequestDetail(request.Username, issued.IssuedGroupLines.ToList(),
                //        ref fabricRequest,
                //        ref fabricRequestDetails);

                //    context.FabricRequest.Update(fabricRequest);
                //    context.FabricRequestDetail.UpdateRange(fabricRequestDetails);
                //}

                context.StorageDetail.UpdateRange(currentStorageDetails);
                context.MaterialTransaction.AddRange(transactions);
                context.Issued.Add(issued.Value);

            }

            context.EntitySequenceNumber.Update(sequenceNumber);
            context.SaveChanges();

            try
            {
                return Task.CompletedTask;
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("{@time} - Create issued when import storage detail event handler has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Create issued when import storage detail event handler has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
                return Task.CompletedTask;
            }
        }
    }
}
