using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Events;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
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
    public class UpdateQuantityCommandHandler : IRequestHandler<UpdateQuantityCommand, UpdateQuantityResult>
    {
        private readonly ILogger<UpdateQuantityCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IMediator mediator;

        public UpdateQuantityCommandHandler(ILogger<UpdateQuantityCommandHandler> logger,
            SqlServerAppDbContext context,
            ISalesOrderRepository salesOrderRepository,
            IItemStyleRepository itemStyleRepository,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.salesOrderRepository = salesOrderRepository;
            this.itemStyleRepository = itemStyleRepository;
            this.mediator = mediator;
        }

        public async Task<UpdateQuantityResult> Handle(UpdateQuantityCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute update quantity for sales order command", DateTime.Now.ToString());
            var result = new UpdateQuantityResult();

            if (!FileHelpers.SaveFile(request.UpdateFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var salesOrders = salesOrderRepository
                .GetSalesOrders(request.CustomerID);

            var updateResult = UpdateQuantityProcess
                .UpdateMultiSalesOrder(salesOrders, 
                     request.Username, fullPath, 
                     out List<OrderDetail> orderDetails, out List<ItemStyleBarCode> itemStyleBarcodes, 
                     out string errorMessage);
            
            if (!updateResult)
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                context.OrderDetail.UpdateRange(orderDetails);
                context.ItemStyleBarCode.UpdateRange(itemStyleBarcodes);
                context.SaveChanges();
                result.IsSuccess = true;

                await mediator.Publish(new OrderDetailBulkUpdatedEvent()
                {
                    UpdatedOrderDetails = orderDetails,
                    UserName = request.Username
                });
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                LogHelper.Instance.Error("{@time} - Exceute update quantity for sales order command with message {@message}",
                    DateTime.Now.ToString(), ex.InnerException?.Message);
            }
            finally
            {
                var jobId = BackgroundJob.Enqueue<UpdateShipQuantityJob>(j => j.Execute(orderDetails.Select(x => x.ItemStyleNumber).ToList()));
            }
            return result;
        }
    }
}
