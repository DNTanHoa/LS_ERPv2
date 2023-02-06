using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
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
    public class UpdatePaymentTermCommandHandler
        : IRequestHandler<UpdatePaymentTermCommand, CommonCommandResult<PaymentTerm>>
    {
        private readonly ILogger<UpdatePaymentTermCommandHandler> logger;
        private readonly IPaymentTermRepository paymentTermRepository;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public UpdatePaymentTermCommandHandler(ILogger<UpdatePaymentTermCommandHandler> logger,
            IPaymentTermRepository paymentTermRepository,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.paymentTermRepository = paymentTermRepository;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CommonCommandResult<PaymentTerm>> Handle(UpdatePaymentTermCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<PaymentTerm>();

            var existPaymentTerm = paymentTermRepository.GetPaymentTerm(request.Code);

            if(existPaymentTerm == null)
            {
                result.Message = "Not found PaymentTerm";
                return result;
            }
            
            mapper.Map(request, existPaymentTerm);
            existPaymentTerm.SetUpdateAudit(request.GetUser());

            try
            {
                paymentTermRepository.Update(existPaymentTerm);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = existPaymentTerm;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
