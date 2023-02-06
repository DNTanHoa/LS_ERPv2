using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class CreateShippingPlanCommandHandler
        : IRequestHandler<CreateShippingPlanCommand, CommonCommandResult>
    {
        private readonly ILogger<CreateShippingPlanCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public CreateShippingPlanCommandHandler(ILogger<CreateShippingPlanCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task<CommonCommandResult> Handle(
            CreateShippingPlanCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                var shippingPlan = mapper.Map<ShippingPlan>(request);
                shippingPlan.SetCreateAudit(request.UserName);

                context.ShippingPlans.Add(shippingPlan);
                context.SaveChangesAsync();

                result.Success = true;
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
