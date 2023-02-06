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
    public class CreateCurrencyCommandHandler :
        IRequestHandler<CreateCurrencyCommand, CommonCommandResult<Currency>>
    {
        private readonly ILogger<CreateCurrencyCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly ICurrencyRepository currencyRepository;
        private readonly SqlServerAppDbContext context;

        public CreateCurrencyCommandHandler(ILogger<CreateCurrencyCommandHandler> logger,
            IMapper mapper,
            ICurrencyRepository currencyRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.currencyRepository = currencyRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Currency>> Handle(CreateCurrencyCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Currency>();
            var currency = mapper.Map<Currency>(request);
            currency.SetCreateAudit(request.GetUser());

            try
            {
                currency = currencyRepository.Add(currency);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = currency;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
