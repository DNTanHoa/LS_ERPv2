using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
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

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ProcessMaterialRequestCommandHandler
        : IRequestHandler<ProcessMaterialRequestCommand, CommonCommandResult>
    {
        private readonly ILogger<ProcessMaterialRequestCommandHandler> logger;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public ProcessMaterialRequestCommandHandler(ILogger<ProcessMaterialRequestCommandHandler> logger,
            IEntitySequenceNumberRepository entitySequenceNumberRepository,
            IStorageDetailRepository storageDetailRepository,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
            this.storageDetailRepository = storageDetailRepository;
            this.mapper = mapper;
            this.context = context;
        }
        public Task<CommonCommandResult> Handle(
            ProcessMaterialRequestCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            logger.LogInformation("{@time} - Exceute process material request command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute process material request command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var materialRequests = context.MaterialRequests
                    .Include(x => x.Details)
                    .Where(x => request.MaterialRequestIds.Contains(x.Id))
                    .ToList();

                if(materialRequests.Any())
                {
                    foreach(var materialRequest in materialRequests)
                    {
                        if(materialRequest.IsProcess != true ||
                           materialRequest.IsSuccess != true)
                        {
                            materialRequest.IsProcess = true;

                            if(materialRequest.IsSuccess != true)
                            {
                                /// Validate information
                                if (string.IsNullOrEmpty(materialRequest.CustomerID))
                                {
                                    materialRequest.IsSuccess = false;
                                    materialRequest.ErrorMessage = "Customer is empty";
                                    continue;
                                }

                                if (string.IsNullOrEmpty(materialRequest.StorageCode))
                                {
                                    materialRequest.IsSuccess = false;
                                    materialRequest.ErrorMessage = "Storage code is empty";
                                    continue;
                                }

                                /// Create issued
                                var issued = new Issued()
                                {
                                    Description = materialRequest.RequestFor,
                                    Remark = materialRequest.LSStyles,
                                    StorageCode = materialRequest.StorageCode,
                                    CustomerID = materialRequest.CustomerID,
                                    IssuedDate = materialRequest.RequestDate, /// To keep request date
                                    IssuedBy = request.UserName,
                                };

                                issued.Number = entitySequenceNumberRepository.GetNextNumberByCode("Issued",
                                    out EntitySequenceNumber sequenceNumber);
                                issued.SetCreateAudit(request.UserName);

                                var issuedLines = new List<IssuedLine>();
                                
                                /// Create issued groupline and line
                                foreach(var materialRequestDetail in materialRequest.Details)
                                {
                                    var issuedLine = mapper.Map<IssuedLine>(materialRequestDetail);
                                    issuedLine.IssuedNumber = issued.Number;
                                    issuedLines.Add(issuedLine);
                                }

                                var issuedLinesGroup = issuedLines.GroupBy(x => new
                                {
                                    x.IssuedNumber,
                                    x.ItemID,
                                    x.ItemName,
                                    x.ItemColorCode,
                                    x.ItemColorName,
                                    x.GarmentColorCode,
                                    x.GarmentColorName,
                                    x.CustomerStyle,
                                    x.Specify,
                                    x.ItemCode,
                                    x.Season,
                                    x.UnitID
                                });

                                foreach(var group in issuedLinesGroup)
                                {
                                    var issuedGroupLine = new IssuedGroupLine()
                                    {
                                        IssuedNumber = group.Key.IssuedNumber,
                                        ItemCode = group.Key.ItemCode,
                                        ItemName = group.Key.ItemName,
                                        ItemID = group.Key.ItemID,
                                        ItemColorCode = group.Key.ItemColorCode,
                                        ItemColorName = group.Key.ItemColorName,
                                        Specify = group.Key.Specify,
                                        Season = group.Key.Season,
                                        UnitID = group.Key.UnitID,
                                        GarmentColorCode = group.Key.GarmentColorCode,
                                        GarmentColorName = group.Key.GarmentColorName,
                                        CustomerStyle = group.Key.CustomerStyle,
                                        IssuedQuantity = group.Sum(x => x.IssuedQuantity) ?? 0,
                                        IssuedLines = group.Select(x => x).ToList()
                                    };

                                    issued.IssuedGroupLines.Add(issuedGroupLine);
                                }
                                issued.Remark += string.Join(";", issued.IssuedLines.Select(x => x.LSStyle).Distinct());

                                ///Create material transactions
                                var transactions = MaterialTransactionProcess
                                    .CreateTransactions(issued, request.UserName);

                                ///Canculate and update storage detail
                                //var currentStorageDetails = storageDetailRepository
                                //    .GetStorageDetailsForCustomer(materialRequest.StorageCode, materialRequest.CustomerID);
                                //var updateStorageDetailResult = StorageDetailProcess.UpdateStorageFromIssued(currentStorageDetails.ToList(),
                                //    transactions, materialRequest.StorageCode, request.UserName, out string errorMesssage);

                                materialRequest.IsProcess = true;
                                materialRequest.IsSuccess = true;

                                context.MaterialRequests.Update(materialRequest);
                                //context.StorageDetail.UpdateRange(currentStorageDetails);
                                context.MaterialTransaction.AddRange(transactions);
                                context.Issued.Add(issued);
                                context.EntitySequenceNumber.Update(sequenceNumber);

                                context.SaveChanges();

                                
                            }
                        }
                    }
                }

                context.SaveChanges();
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;

                logger.LogInformation("{@time} - Exceute process material request command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.Message);
                LogHelper.Instance.Information("{@time} - Exceute process material request command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.Message);
            }

            return Task.FromResult(result);
        }
    }
}
