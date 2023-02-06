using AutoMapper;
using Common.Model;
using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class CreatePartPriceCommandHandler
        : IRequestHandler<CreatePartPriceCommand, CommonCommandResultHasData<PartPrice>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreatePartPriceCommandHandler> logger;
        private readonly IMapper mapper;

        public CreatePartPriceCommandHandler(SqlServerAppDbContext context,
            ILogger<CreatePartPriceCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<PartPrice>> Handle(CreatePartPriceCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<PartPrice>();
            logger.LogInformation("{@time} - Exceute create part price command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create part price command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            var existPartPrice = context.PartPrice.FirstOrDefault(x => x.StyleNO == request.StyleNO
                                                               && x.Item == request.Item
                                                               && x.CompanyID == request.CompanyID);

            if (existPartPrice == null)
            {
                var partPrice = mapper.Map<PartPrice>(request);
                try
                {
                    partPrice.SetCreateAudit(request.UserName);
                    context.PartPrice.Add(partPrice);
                    context.SaveChanges();
                    result.Success = true;
                    result.SetData(partPrice);
                }
                catch (DbUpdateException ex)
                {
                    result.Message = ex.Message;
                    logger.LogInformation("{@time} - Exceute create part price command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                    LogHelper.Instance.Information("{@time} - Exceute create part price command with request {@request} has error {@error}",
                        DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                }
            }

            return Task.FromResult(result);
        }

    }
}
