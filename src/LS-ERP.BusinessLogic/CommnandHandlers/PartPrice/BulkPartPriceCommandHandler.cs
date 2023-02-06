using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class BulkPartPriceCommandHandler : IRequestHandler<BulkPartPriceCommand,
        CommonCommandResultHasData<IEnumerable<PartPrice>>>
    {
        private readonly ILogger<BulkPartPriceCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public BulkPartPriceCommandHandler(ILogger<BulkPartPriceCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<IEnumerable<PartPrice>>> Handle(
            BulkPartPriceCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<IEnumerable<PartPrice>>();
            logger.LogInformation("{@time} - Exceute bulk part price command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute bulk part price command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            if (request.Data != null)
            {
                if (request.Data.Count > 0)
                {
                    request.UserName = request.Data.FirstOrDefault().UserName;
                }
            }
            try
            {
                var partPrices = request.Data
                    .Select(x => mapper.Map<PartPrice>(x)).ToList();

                foreach (var partPrice in partPrices)
                {
                    var existpartPrice = context.PartPrice.Where(x => x.StyleNO == partPrice.StyleNO
                                                              && x.Item == partPrice.Item).FirstOrDefault();
                    if (existpartPrice != null)
                    {
                        context.PartPrice.Remove(existpartPrice);
                    }
                    partPrice.SetCreateAudit(request.UserName);
                    context.PartPrice.Add(partPrice);
                    context.SaveChanges();
                }
                result.Success = true;
                result.Data = partPrices;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute bulk part price command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute bulk part price command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
