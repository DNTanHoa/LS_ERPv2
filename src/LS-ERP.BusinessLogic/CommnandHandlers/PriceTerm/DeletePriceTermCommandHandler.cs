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
    public class DeletePriceTermCommandHandler :
        IRequestHandler<DeletePriceTermCommand, CommonCommandResult<PriceTerm>>
    {
        private readonly ILogger<DeletePriceTermCommandHandler> logger;
        private readonly IPriceTermRepository priceTermRepository;
        private readonly SqlServerAppDbContext context;

        public DeletePriceTermCommandHandler(ILogger<DeletePriceTermCommandHandler> logger,
            IPriceTermRepository priceTermRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.priceTermRepository = priceTermRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<PriceTerm>> Handle(DeletePriceTermCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<PriceTerm>();

            var existPriceTerm = priceTermRepository.GetPriceTerm(request.Code);

            if (existPriceTerm == null)
            {
                result.Message = "Not found PriceTerm";
                return result;
            }

            try
            {
                priceTermRepository.Delete(existPriceTerm);
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
