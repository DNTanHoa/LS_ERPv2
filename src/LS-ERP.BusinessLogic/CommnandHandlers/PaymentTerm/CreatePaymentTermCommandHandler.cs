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
    public class CreatePaymentTermCommandHandler :
        IRequestHandler<CreatePaymentTermCommand, CommonCommandResult<PaymentTerm>>
    {
        private readonly ILogger<CreatePaymentTermCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly IPaymentTermRepository paymentTermRepository;
        private readonly SqlServerAppDbContext context;

        public CreatePaymentTermCommandHandler(ILogger<CreatePaymentTermCommandHandler> logger,
            IMapper mapper,
            IPaymentTermRepository paymentTermRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.paymentTermRepository = paymentTermRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<PaymentTerm>> Handle(CreatePaymentTermCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<PaymentTerm>();
            var paymentTerm = mapper.Map<PaymentTerm>(request);
            paymentTerm.SetCreateAudit(request.GetUser());

            try
            {
                paymentTerm = paymentTermRepository.Add(paymentTerm);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = paymentTerm;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
