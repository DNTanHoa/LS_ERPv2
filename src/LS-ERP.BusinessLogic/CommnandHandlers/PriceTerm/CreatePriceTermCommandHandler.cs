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
    public class CreatePriceTermCommandHandler :
        IRequestHandler<CreatePriceTermCommand, CommonCommandResult<PriceTerm>>
    {
        private readonly ILogger<CreatePriceTermCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly IPriceTermRepository priceTermRepository;
        private readonly SqlServerAppDbContext context;

        public CreatePriceTermCommandHandler(ILogger<CreatePriceTermCommandHandler> logger,
            IMapper mapper,
            IPriceTermRepository priceTermRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.priceTermRepository = priceTermRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<PriceTerm>> Handle(CreatePriceTermCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<PriceTerm>();
            var priceTerm = mapper.Map<PriceTerm>(request);
            priceTerm.SetCreateAudit(request.GetUser());

            try
            {
                priceTerm = priceTermRepository.Add(priceTerm);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = priceTerm;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
