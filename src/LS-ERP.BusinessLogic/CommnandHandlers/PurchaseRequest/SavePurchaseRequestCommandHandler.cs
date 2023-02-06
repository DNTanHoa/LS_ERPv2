using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class SavePurchaseRequestCommandHandler
        : IRequestHandler<SavePurchaseRequestCommand, CommonCommandResult<PurchaseRequest>>
    {
        private readonly ILogger<SavePurchaseRequestCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly IPurchaseRequestRepository purchaseRequestRepository;
        private readonly PurchaseRequestValidator validator;
        private readonly SqlServerAppDbContext context;

        public SavePurchaseRequestCommandHandler(ILogger<SavePurchaseRequestCommandHandler> logger,
            IMapper mapper,
            IPurchaseRequestRepository purchaseRequestRepository,
            PurchaseRequestValidator validator,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.purchaseRequestRepository = purchaseRequestRepository;
            this.validator = validator;
            this.context = context;
        }

        public async Task<CommonCommandResult<PurchaseRequest>> Handle(SavePurchaseRequestCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<PurchaseRequest>();

            logger.LogInformation("{@time} - Exceute create purchase request command",
                DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create purchase request command",
                DateTime.Now.ToString());

            PurchaseRequest purchaseRequest = null;

            if (purchaseRequestRepository.IsExist(request.ID))
            {
                purchaseRequest = purchaseRequestRepository.GetPurchaseRequest(request.ID);
                mapper.Map(request, purchaseRequest);
                purchaseRequest.SetUpdateAudit(request.Username);

                if (!validator.IsValid(purchaseRequest, out string errorMessage))
                {
                    result.Message = errorMessage;
                    return result;
                }

                foreach (var purchaseRequestGroupLine in purchaseRequest.PurchaseRequestGroupLines)
                {
                    foreach (var purchaseRequestLine in purchaseRequestGroupLine.PurchaseRequestLines)
                    {
                        purchaseRequestLine.RemainQuantity =
                        (purchaseRequestLine.Quantity - (purchaseRequestLine.PurchasedQuantity ?? 0));
                    }
                }

                context.PurchaseRequest.Update(purchaseRequest);
            }
            else
            {
                purchaseRequest = mapper.Map<PurchaseRequest>(request);
                purchaseRequest.SetCreateAudit(request.Username);

                if (!validator.IsValid(purchaseRequest, out string errorMessage))
                {
                    result.Message = errorMessage;
                    return result;
                }

                foreach (var purchaseRequestGroupLine in purchaseRequest.PurchaseRequestGroupLines)
                {
                    foreach(var purchaseRequestLine in purchaseRequestGroupLine.PurchaseRequestLines)
                    {
                        purchaseRequestLine.RemainQuantity =
                        (purchaseRequestLine.Quantity - (purchaseRequestLine.PurchasedQuantity ?? 0));
                    }
                }

                context.PurchaseRequest.Add(purchaseRequest);
            }

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = purchaseRequest;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException?.Message;
            }

            return result;
        }
    }
}
