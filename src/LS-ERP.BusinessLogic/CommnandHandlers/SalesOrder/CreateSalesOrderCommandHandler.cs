using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class CreateSalesOrderCommandHandler
        : IRequestHandler<CreateSalesOrderCommand, CommonCommandResult<SalesOrder>>
    {
        private readonly ILogger<CreateSalesOrderCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly SalesOrderValidator salesOrderValidator;
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly SqlServerAppDbContext context;

        public CreateSalesOrderCommandHandler(ILogger<CreateSalesOrderCommandHandler> logger,
            IMapper mapper,
            SalesOrderValidator salesOrderValidator,
            ISalesOrderRepository salesOrderRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.salesOrderValidator = salesOrderValidator;
            this.salesOrderRepository = salesOrderRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<SalesOrder>> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<SalesOrder>();
            var salesOrder = mapper.Map<SalesOrder>(request);

            if(!salesOrderValidator.IsValid(salesOrder, out string errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            salesOrder.SetCreateAudit(request.GetUser());

            try
            {
                salesOrder = salesOrderRepository.Add(salesOrder);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = salesOrder;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
