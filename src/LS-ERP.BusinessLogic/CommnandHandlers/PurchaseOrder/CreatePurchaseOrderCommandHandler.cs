using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Events;
using LS_ERP.BusinessLogic.Shared.Process;
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
    public class CreatePurchaseOrderCommandHandler
        : IRequestHandler<CreatePurchaseOrderCommand, CommonCommandResult<PurchaseOrder>>
    {
        private readonly ILogger<CreatePurchaseOrderCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IProductionBOMRepository productionBOMRepository;
        private readonly PurchaseOrderValidator validator;
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public CreatePurchaseOrderCommandHandler(ILogger<CreatePurchaseOrderCommandHandler> logger,
            SqlServerAppDbContext context,
            IProductionBOMRepository productionBOMRepository,
            PurchaseOrderValidator validator,
            IMediator mediator,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.productionBOMRepository = productionBOMRepository;
            this.validator = validator;
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public async Task<CommonCommandResult<PurchaseOrder>> Handle(CreatePurchaseOrderCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<PurchaseOrder>();

            logger.LogInformation("{@time} - Exceute create purchase order command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create purchase order command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            var purchaseOrder = mapper.Map<PurchaseOrder>(request);

            if(!validator.IsValid(purchaseOrder, out string erroorMessage))
            {
                result.Message = erroorMessage;
                return result;
            }

            purchaseOrder.SetCreateAudit(request.GetUser());

            try
            {
                context.PurchaseOrder.Add(purchaseOrder);
                context.SaveChanges();

                await mediator.Publish(new PurchaseOrderCreatedEvent()
                {
                    PurchaseOrderNumber = purchaseOrder.Number,
                    UserName = request.Username
                });

                result.IsSuccess = true;
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
