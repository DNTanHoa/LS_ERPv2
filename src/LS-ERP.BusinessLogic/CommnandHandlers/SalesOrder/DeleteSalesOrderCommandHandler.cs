using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeleteSalesOrderCommandHandler
        : IRequestHandler<DeleteSalesOrderCommand, CommonCommandResult<SalesOrder>>
    {
        private readonly ILogger<DeleteSalesOrderCommandHandler> logger;
        private readonly AppDbContext context;

        public DeleteSalesOrderCommandHandler(ILogger<DeleteSalesOrderCommandHandler> logger,
            AppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public async Task<CommonCommandResult<SalesOrder>> Handle(
            DeleteSalesOrderCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("--- Execute delete sales order command at {@time} with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            LogHelper.Instance.Information("--- Execute delete sales order command at {@time} with request {@request}",
                DateTime.Now.ToString(), JsonConvert.SerializeObject(request));

            var result = new CommonCommandResult<SalesOrder>();

            var salesOrder = context.SalesOrders.FirstOrDefault(x => x.ID == request.ID);
            
            var itemStyles = context.ItemStyle.Where(x => x.SalesOrderID == request.ID);
            
            
            var itemStyleNumbers = itemStyles.Select(x => x.Number).ToList();

            var itemStyleSyncMasters = context.ItemStyleSyncMasters
                .Where(x => itemStyleNumbers.Contains(x.ItemStyleNumber));
            var orderDetails = context.OrderDetail
                .Where(x => itemStyleNumbers.Contains(x.ItemStyleNumber));
            var itemStyleBarCodes = context.ItemStyleBarCode
                .Where(x => itemStyleNumbers.Contains(x.ItemStyleNumber));


            if (salesOrder == null)
            {
                result.Message = "Can't find sales order to delete";
                return result;
            }

            if(orderDetails.Where(x => x.ReservedQuantity > 0).Any())
            {
                result.Message = "Sales order has item style bom pulled. Can't delete";
                return result;
            }

            try
            {
                context.ItemStyleSyncMasters.RemoveRange(itemStyleSyncMasters);
                context.ItemStyleBarCode.RemoveRange(itemStyleBarCodes);
                context.OrderDetail.RemoveRange(orderDetails);
                context.ItemStyle.RemoveRange(itemStyles);
                context.SalesOrders.Remove(salesOrder);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                return result;
            }
            catch (DbUpdateException ex)
            {
                logger.LogInformation("--- Execute delete sales order command at {@time} has error {@message}",
                DateTime.Now.ToString(), ex.Message);
                LogHelper.Instance.Information("--- Execute delete sales order command at {@time} has error {@message}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(request));
            }

            return result;
        }
    }
}
