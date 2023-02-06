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
    public class CreateIssuedCommandHandler
        : IRequestHandler<CreateIssuedCommand, CommonCommandResult<Issued>>
    {
        private readonly ILogger<CreateIssuedCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;
        private readonly IStorageDetailRepository storageDetailRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;

        public CreateIssuedCommandHandler(ILogger<CreateIssuedCommandHandler> logger,
            IMapper mapper,
            SqlServerAppDbContext context,
            IStorageDetailRepository storageDetailRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.context = context;
            this.storageDetailRepository = storageDetailRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
        }

        public async Task<CommonCommandResult<Issued>> Handle(CreateIssuedCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<Issued>();

            logger.LogInformation("{@time} - Exceute create issued command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create issued command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

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
                .CreateTransactions(issued, request.GetUser());

            ///Canculate and update storage detail
            //var currentStorageDetails = storageDetailRepository
            //    .GetStorageDetailsForCustomer(request.StorageCode, request.CustomerID);
            //var updateStorageDetailResult = StorageDetailProcess.UpdateStorageFromIssued(currentStorageDetails.ToList(),
            //    transactions, request.StorageCode, request.Username, out string errorMesssage);

            //if(!updateStorageDetailResult)
            //{
            //    result.Message = errorMesssage;
            //    return result;
            //}

            try
            {
                //context.StorageDetail.UpdateRange(currentStorageDetails);
                context.MaterialTransaction.AddRange(transactions);
                context.Issued.Add(issued);
                context.EntitySequenceNumber.Update(sequenceNumber);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch(DbUpdateException ex)
            {
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
