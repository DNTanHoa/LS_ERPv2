using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class BulkShippingPlanCommandHandler : IRequestHandler<BulkShippingPlanCommand, CommonCommandResult>
    {
        private readonly ILogger<BulkShippingPlanCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public BulkShippingPlanCommandHandler(ILogger<BulkShippingPlanCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.mediator = mediator;
        }
        public Task<CommonCommandResult> Handle(BulkShippingPlanCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                var filePath = request.Data.FirstOrDefault().FilePath;
                var idx = filePath.LastIndexOf('/') + 1;
                var shippingPlan = new ShippingPlan()
                {
                    CompanyID = request.CompanyID,
                    CustomerID = request.CustomerID,
                    FilePath = filePath,
                    Title = filePath.Substring(idx, filePath.Length - idx - 22) + ".xlsx",
                    Details = request.Data
                };

                shippingPlan.SetCreateAudit(request.UserName);

                context.ShippingPlans.AddRange(shippingPlan);
                context.SaveChanges();

                result.Success = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
