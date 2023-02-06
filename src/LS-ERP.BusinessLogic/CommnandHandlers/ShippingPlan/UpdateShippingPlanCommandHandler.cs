using AutoMapper;
using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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
    public class UpdateShippingPlanCommandHandler
        : IRequestHandler<UpdateShippingPlanCommand, CommonCommandResult>
    {
        private readonly ILogger<UpdateShippingPlanCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public UpdateShippingPlanCommandHandler(ILogger<UpdateShippingPlanCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }
        public Task<CommonCommandResult> Handle(
            UpdateShippingPlanCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                var shippingPlan = context.ShippingPlans
                    .Include(x => x.Details)
                    .FirstOrDefault(x => x.ID == request.Id);

                if(shippingPlan != null)
                {
                    mapper.Map(request, shippingPlan);
                    shippingPlan.SetUpdateAudit(request.UserName);

                    foreach(var detail in request.Details)
                    {
                        var preDetail = shippingPlan.Details
                            .FirstOrDefault(x => x.LSStyle == detail.LSStyle);

                        if(preDetail != null)
                        {
                            preDetail.CTN = detail.CTN;
                            preDetail.PCS = detail.PCS;
                            preDetail.PurchaseOrderNumber = detail.PurchaseOrderNumber;
                            preDetail.Volume = detail.Volume;
                            preDetail.Dept = detail.Dept;
                            preDetail.GrossWeight = detail.GrossWeight;
                        }
                        else
                        {
                            shippingPlan.Details.Add(detail);
                        }
                    }

                    foreach(var detail in shippingPlan.Details)
                    {
                        var newDetail = request.Details
                            .FirstOrDefault(x => x.LSStyle == detail.LSStyle);

                        if (newDetail == null)
                            detail.IsDeActive = true;
                    }

                    context.ShippingPlans.Update(shippingPlan);
                    context.SaveChanges();

                    result.Success = true;
                }
                else
                {
                    result.Message = "Not found shipping to update";
                }
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
            }

            return Task.FromResult(result);
        }
    }
}
