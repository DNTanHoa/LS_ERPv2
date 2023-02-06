using AutoMapper;
using LS_ERP.BusinessLogic.Events;
using LS_ERP.EntityFrameworkCore;
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

namespace LS_ERP.BusinessLogic.EventHandlers
{
    public class ItemStyleBulkedEventHandler : INotificationHandler<ItemStyleBulkedEvent>
    {
        private readonly ILogger<ItemStyleBulkedEventHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public ItemStyleBulkedEventHandler(ILogger<ItemStyleBulkedEventHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public Task Handle(ItemStyleBulkedEvent notification, CancellationToken cancellationToken)
        {
            var itemStyles = context.ItemStyle
                .Include(x => x.OrderDetails)
                .Include(x => x.SalesOrder)
                .Where(x => notification.ItemStyleNumbers.Contains(x.Number)).ToList();
            var itemStylesSyncActions = new List<ItemStyleSyncAction>();

            if(itemStyles != null &&
               itemStyles.Any())
            {
                var itemStyleSyncMasters = new List<ItemStyleSyncMaster>();
                itemStyles.ForEach(x =>
                {
                    /// Sum total quantity
                    x.TotalQuantity = x.OrderDetails?.Sum(d => d.Quantity);
                    /// Sync to PPC with status wait for confirm
                    var itemStyleSyncMaster = mapper.Map<ItemStyleSyncMaster>(x);

                    itemStyleSyncMaster.Monthly = DateTime.Now.ToString("MMMM");

                    var itemStyleSyncAction = new ItemStyleSyncAction()
                    {
                        Action= "Created",
                        ActionDate = DateTime.Now
                    };

                    itemStyleSyncMaster.ItemStyleSyncActions.Add(itemStyleSyncAction);

                    if(x.OrderDetails != null &&
                       x.OrderDetails.Any())
                    {
                        foreach(var orderDetail in x.OrderDetails)
                        {
                            var orderDetailSync = new OrderDetailSync()
                            {
                                GarmentSize = orderDetail.Size,
                                Quantity = orderDetail.Quantity ?? 0,
                            };

                            itemStyleSyncMaster.OrderDetailSyncs.Add(orderDetailSync);
                        }
                    }

                    itemStyleSyncMasters.Add(itemStyleSyncMaster);
                });

                /// Email
                /// TODO: Send mail when update infor
                try
                {
                    context.ItemStyle.UpdateRange(itemStyles);
                    context.ItemStyleSyncMasters.AddRangeAsync(itemStyleSyncMasters);
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError("ItemStyle bulk event handler has error with message {@message}",
                        ex.InnerException?.Message);
                    LogHelper.Instance.Error("ItemStyle bulk event handler has error with message {@message}",
                        ex.InnerException?.Message);
                }
            }
            return Task.CompletedTask;
        }
    }
}
