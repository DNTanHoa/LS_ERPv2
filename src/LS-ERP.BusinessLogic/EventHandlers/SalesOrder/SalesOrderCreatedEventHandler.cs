using LS_ERP.BusinessLogic.Events;
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

namespace LS_ERP.BusinessLogic.EventHandlers
{
    public class SalesOrderCreatedEventHandler
        : INotificationHandler<SalesOrderCreatedEvent>
    {
        private readonly ILogger<SalesOrderCreatedEventHandler> logger;
        private readonly SqlServerAppDbContext context;

        public SalesOrderCreatedEventHandler(ILogger<SalesOrderCreatedEventHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }
        public Task Handle(SalesOrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            var orderDetails = context.OrderDetail
                .Include(x => x.ItemStyle)
                .Where(x => notification.SalesOrderID.Contains(x.ItemStyle.SalesOrderID)).ToList();

            /// Set ShipQuantity = Quantity
            if (orderDetails != null && orderDetails.Any())
            {
                orderDetails.ForEach(x =>
                {
                    x.ShipQuantity = x.Quantity;
                });
            }

            /// Update handle
            try
            {
                context.OrderDetail.UpdateRange(orderDetails);
                context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.LogError("Sales order event handler has error with message {@message}",
                    ex.InnerException?.Message);
                LogHelper.Instance.Error("Sales order event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;  
        }
    }
}
