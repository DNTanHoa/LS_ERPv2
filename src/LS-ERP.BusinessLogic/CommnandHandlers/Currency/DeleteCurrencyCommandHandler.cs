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
    public class DeleteCurrencyCommandHandler :
        IRequestHandler<DeleteCurrencyCommand, CommonCommandResult<Currency>>
    {
        private readonly ILogger<DeleteCurrencyCommandHandler> logger;
        private readonly ICurrencyRepository currencyRepository;
        private readonly SqlServerAppDbContext context;

        public DeleteCurrencyCommandHandler(ILogger<DeleteCurrencyCommandHandler> logger,
            ICurrencyRepository currencyRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.currencyRepository = currencyRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Currency>> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Currency>();

            var existCurrency = currencyRepository.GetCurrency(request.ID);

            if (existCurrency == null)
            {
                result.Message = "Not found Currency";
                return result;
            }

            try
            {
                currencyRepository.Delete(existCurrency);
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
