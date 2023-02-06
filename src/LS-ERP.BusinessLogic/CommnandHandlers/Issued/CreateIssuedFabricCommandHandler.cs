using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class CreateIssuedFabricCommandHandler
        : IRequestHandler<CreateIssuedFabricCommand, CommonCommandResult<Issued>>
    {
        private readonly ILogger<CreateIssuedFabricCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IFabricRequestDetailRepository fabricRequestDetailRepository;
        private readonly IFabricRequestRepository fabricRequestRepository;
        private readonly IFabricPurchaseOrderRepository fabricPurchaseOrderRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;

        public CreateIssuedFabricCommandHandler(ILogger<CreateIssuedFabricCommandHandler> logger,
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

        public async Task<CommonCommandResult<Issued>> Handle(CreateIssuedFabricCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<Issued>();

            logger.LogInformation("{@time} - Exceute create issued fabric command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create issued fabric command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            //using var transaction = context.Database.BeginTransaction();
            //transaction.CreateSavepoint("SaveIssuedFabric");

            try
            {
                ///Map issued
                var issued = mapper.Map<Issued>(request);
                issued.Number = entitySequenceNumberRepository.GetNextNumberByCode("Issued",
                    out EntitySequenceNumber sequenceNumber);
                issued.SetCreateAudit(request.Username);

                ///Map issued line
                issued.IssuedGroupLines
                    .SelectMany(x => x.IssuedLines).ToList().ForEach(x =>
                    {
                        x.IssuedNumber = issued.Number;
                    });

                ///Create material transactions
                var transactions = MaterialTransactionProcess
                    .CreateTransactionsFB(issued, request.GetUser());

                ///Canculate and update storage detail
                var currentStorageDetails = storageDetailRepository
                    .GetStorageDetails(request.IssuedGroupLines.Select(x => x.StorageDetailID).ToList());

                var updateStorageDetailResult = StorageDetailProcess.UpdateFabricStorageFromIssued(currentStorageDetails.ToList(),
                    transactions, request.StorageCode, request.Username, out string errorMesssage);

                if (!updateStorageDetailResult)
                {
                    result.Message = errorMesssage;
                    return result;
                }
                ///Update Fabric purchase order information data
                //if (!string.IsNullOrEmpty(request.ProductionMethodCode) && request.ProductionMethodCode.Equals("CMT"))
                //{
                var fabricPurchaseOrders = fabricPurchaseOrderRepository
               .GetFabricPurchaseOrders(currentStorageDetails.Select(x => x.FabricPurchaseOrderNumber).ToList());

                if (fabricPurchaseOrders.Any())
                {
                    ImportFabricPurchaseOrderProcessor.CalculateIssuedQuantity(fabricPurchaseOrders.ToList(), currentStorageDetails.ToList(),
                                     issued?.IssuedGroupLines.ToList(),
                                     out string errorMessage);

                    context.FabricPurchaseOrder.UpdateRange(fabricPurchaseOrders);
                }

                //}

                if (request.FabricRequestID != null && request.FabricRequestID != 0)
                {
                    var fabricRequestDetails = fabricRequestDetailRepository.GetFabricRequestDetails((long)request.FabricRequestID)?.ToList();
                    var fabricRequest = fabricRequestRepository.GetFabricRequest(request.FabricRequestID);

                    FabricRequestProcessor.UpdateFabricRequestDetail(request.Username, issued.IssuedGroupLines.ToList(),
                        ref fabricRequest,
                        ref fabricRequestDetails);

                    context.FabricRequest.Update(fabricRequest);
                    context.FabricRequestDetail.UpdateRange(fabricRequestDetails);
                }
                else
                {
                    issued.FabricRequestID = null;
                }


                context.StorageDetail.UpdateRange(currentStorageDetails);
                context.MaterialTransaction.AddRange(transactions);

                context.Issued.Add(issued);
                context.EntitySequenceNumber.Update(sequenceNumber);

                context.SaveChanges();

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;

                //transaction.Commit();
            }
            catch (DbUpdateException ex)
            {
                //transaction.RollbackToSavepoint("SaveIssuedFabric");
                logger.LogInformation("{@date}-Create issued for request {@request} error with message {@message}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(request), ex.InnerException?.Message);
                LogHelper.Instance.Error("{@date}-Create issued for request {@request} error with message {@message}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(request), ex.InnerException?.Message);
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
