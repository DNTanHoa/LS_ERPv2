using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
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
    public class CancelOrderDetailCommandHandler
        : IRequestHandler<CancelOrderDetailCommand, CancelOrderDetailResult>
    {
        private readonly ILogger<CancelOrderDetailCommandHandler> logger;
        private readonly IOrderDetailRepository orderDetailRepository;
        private readonly SqlServerAppDbContext context;

        public CancelOrderDetailCommandHandler(ILogger<CancelOrderDetailCommandHandler> logger,
            IOrderDetailRepository orderDetailRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.orderDetailRepository = orderDetailRepository;
            this.context = context;
        }
        public async Task<CancelOrderDetailResult> Handle(
            CancelOrderDetailCommand request, CancellationToken cancellationToken)
        {
            var result = new CancelOrderDetailResult();

            logger.LogInformation("{@time} - Execute cancel order detail command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Execute cancel order detail command",
                DateTime.Now.ToString());

            var orderDetails = orderDetailRepository.GetOrderDetails(request.OrderDetailIDs);

            try
            {
                context.OrderDetail.UpdateRange(orderDetails);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
                logger.LogInformation("{@time} - Execute cancel order detail command", DateTime.Now.ToString());
                LogHelper.Instance.Information("{@time} - Execute cancel order detail command",
                    DateTime.Now.ToString());
            }

            return result;
        }
    }
}
