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
    public class DeletePaymentTermCommandHandler :
        IRequestHandler<DeletePaymentTermCommand, CommonCommandResult<PaymentTerm>>
    {
        private readonly ILogger<DeletePaymentTermCommandHandler> logger;
        private readonly IPaymentTermRepository paymentTermRepository;
        private readonly SqlServerAppDbContext context;

        public DeletePaymentTermCommandHandler(ILogger<DeletePaymentTermCommandHandler> logger,
            IPaymentTermRepository paymentTermRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.paymentTermRepository = paymentTermRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<PaymentTerm>> Handle(DeletePaymentTermCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<PaymentTerm>();

            var existPaymentTerm = paymentTermRepository.GetPaymentTerm(request.Code);

            if (existPaymentTerm == null)
            {
                result.Message = "Not found PaymentTerm";
                return result;
            }

            try
            {
                paymentTermRepository.Delete(existPaymentTerm);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
