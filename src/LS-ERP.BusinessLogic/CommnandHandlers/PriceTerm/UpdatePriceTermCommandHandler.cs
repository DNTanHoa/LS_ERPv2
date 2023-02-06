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
    public class UpdatePriceTermCommandHandler
        : IRequestHandler<UpdatePriceTermCommand, CommonCommandResult<PriceTerm>>
    {
        private readonly ILogger<UpdatePriceTermCommandHandler> logger;
        private readonly IPriceTermRepository PriceTermRepository;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public UpdatePriceTermCommandHandler(ILogger<UpdatePriceTermCommandHandler> logger,
            IPriceTermRepository PriceTermRepository,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.PriceTermRepository = PriceTermRepository;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CommonCommandResult<PriceTerm>> Handle(UpdatePriceTermCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<PriceTerm>();

            var existPriceTerm = PriceTermRepository.GetPriceTerm(request.Code);

            if(existPriceTerm == null)
            {
                result.Message = "Not found PriceTerm";
                return result;
            }
            
            mapper.Map(request, existPriceTerm);
            existPriceTerm.SetUpdateAudit(request.GetUser());

            try
            {
                PriceTermRepository.Update(existPriceTerm);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = existPriceTerm;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
