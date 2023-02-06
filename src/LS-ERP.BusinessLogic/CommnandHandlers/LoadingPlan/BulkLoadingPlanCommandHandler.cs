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
    public class BulkLoadingPlanCommandHandler
         : IRequestHandler<BulkLoadingPlanCommand, CommonCommandResult>
    {
        private readonly ILogger<BulkLoadingPlanCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public BulkLoadingPlanCommandHandler(ILogger<BulkLoadingPlanCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.mediator = mediator;
        }
        public Task<CommonCommandResult> Handle(BulkLoadingPlanCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();
            var loadingPlans = new List<LoadingPlan>();

            try
            {
                foreach (var data in request.Data)
                {
                    var loadingPlan = new LoadingPlan();
                    loadingPlan.ID = data.ID;
                    loadingPlan.ContainerNumber=data.ContainerNumber;
                    loadingPlan.ASNumber=data.ASNumber;
                    loadingPlan.TiersName=data.TiersName;
                    loadingPlan.Shu=data.Shu;
                    loadingPlan.OrderNumber=data.OrderNumber;
                    loadingPlan.ItemCode=data.ItemCode;
                    loadingPlan.PCB=data.PCB;
                    loadingPlan.Port=data.Port;
                    loadingPlan.Rank=data.Rank;
                    loadingPlan.ORINumber=data.ORINumber;
                    loadingPlan.GrossWeight=data.GrossWeight;
                    loadingPlan.NetWeight=data.NetWeight;
                    loadingPlan.Quantity=data.Quantity;
                    loadingPlan.ModelCode=data.ModelCode;
                    loadingPlan.Destination=data.Destination;
                    loadingPlan.Volumn=data.Volumn;
                    loadingPlan.Description=data.Description;
                    loadingPlan.CustomerID=request.CustomerID;
                    loadingPlan.SetCreateAudit(request.UserName);

                    loadingPlans.Add(loadingPlan);
                }

                context.LoadingPlan.AddRange(loadingPlans);
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
