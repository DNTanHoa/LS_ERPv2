using LS_ERP.BusinessLogic.Events;
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
    public class OrderDetailBulkUpdatedEventHandler 
        : INotificationHandler<OrderDetailBulkUpdatedEvent>
    {
        private readonly ILogger<OrderDetailBulkUpdatedEventHandler> logger;
        private readonly SqlServerAppDbContext context;

        public OrderDetailBulkUpdatedEventHandler(ILogger<OrderDetailBulkUpdatedEventHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }
        public Task Handle(OrderDetailBulkUpdatedEvent notification, 
            CancellationToken cancellationToken)
        {
            try
            {
                var orderDetailIDs = notification.UpdatedOrderDetails.Select(x => x.ID).ToList();
                var itemStyleNumbers = notification.UpdatedOrderDetails.Select(x => x.ItemStyleNumber);
                
                ///Lấy danh sách đơn hàng
                var itemStyles = context
                    .ItemStyle
                    .Include(x => x.OrderDetails)
                    .Where(x => itemStyleNumbers.Contains(x.Number)).ToList();

                var itemStyleSyncMasters = context
                    .ItemStyleSyncMasters
                    .Include(x => x.ItemStyleSyncActions)
                    .Where(x => itemStyleNumbers.Contains(x.ItemStyleNumber))
                    .ToList();

                /// Lấy dữ liệu bảng barcode tương ứng
                var itemStyleBarcodes = context.ItemStyleBarCode
                    .Where(x => itemStyleNumbers.Contains(x.ItemStyleNumber))
                    .ToList();

                /// Cập nhật thông tin đơn hàng và phần đồng bộ PCC
                itemStyles.ForEach(x =>
                {
                    var oldTotalQuantity = x.TotalQuantity ?? 0;

                    x.TotalQuantity = x.OrderDetails.Sum(x => x.Quantity);
                    var itemStyleSyncMaster = itemStyleSyncMasters
                        .FirstOrDefault(s => s.ItemStyleNumber == x.Number);

                    if(itemStyleSyncMaster != null)
                    {
                        itemStyleSyncMaster.TotalQuantity = x.TotalQuantity ?? 0;
                        itemStyleSyncMaster.OldTotalQuantity = oldTotalQuantity;

                        var itemStyleSyncAction = new ItemStyleSyncAction()
                        {
                            Action = "Update quantity",
                            ActionDate = DateTime.Now
                        };

                        itemStyleSyncMaster.ItemStyleSyncActions.Add(itemStyleSyncAction);
                    }
                });

                /// Cập nhật thông tin reservation và jobhead liên quan
                var reservationEntries = context
                    .ReservationEntry
                    .Include(x => x.JobHead)
                    .Where(r => r.OrderDetailID != null &&
                                orderDetailIDs.Contains(r.OrderDetailID ?? 0)).ToList();
                
                foreach(var orderDetail in notification.UpdatedOrderDetails)
                {
                    var orderReservations = reservationEntries?
                        .Where(x => x.OrderDetailID == orderDetail.ID)
                        .ToList();

                    if (orderReservations != null &&
                        orderReservations.Any())
                    {
                        var oldTotalReservedQuantity = orderReservations?.Sum(x => x.ReservedQuantity) ?? 0;

                        foreach(var orderReservation in orderReservations)
                        {
                            if(oldTotalReservedQuantity > 0)
                            {
                                orderReservation.ReservedQuantity =
                                    (orderDetail.Quantity - (orderDetail.ConsumedQuantity ?? 0))
                                    * orderReservation.ReservedQuantity / oldTotalReservedQuantity;
                                if (orderReservation.JobHead != null)
                                {
                                    orderReservation.JobHead.ProductionQuantity =
                                        orderReservation.ReservedQuantity;
                                }
                            }
                        }

                        orderDetail.ReservedQuantity = orderDetail.Quantity - (orderDetail.ConsumedQuantity ?? 0);

                        if(itemStyleBarcodes != null)
                        {
                            var itemStyleBarCode = itemStyleBarcodes
                                .FirstOrDefault(x => x.ItemStyleNumber == orderDetail.ItemStyleNumber &&
                                                 x.Size?.ToUpper().Replace(" ","").Trim() == orderDetail.Size?.ToUpper().Replace(" ", "").Trim());

                            if (itemStyleBarCode != null)
                            {
                                itemStyleBarCode.Quantity = orderDetail.Quantity ?? 0;
                            }
                        }
                    }
                }
                

                /// Cập nhật thông tin probom sau khi thay đổi số lượng
                var jobHeadNumbers = reservationEntries.Select(x => x.JobHeadNumber);
                var productionBOMs = context.ProductionBOM
                    .Where(x => jobHeadNumbers.Contains(x.JobHeadNumber)).ToList();
                
                if(productionBOMs != null)
                {
                    productionBOMs.ForEach(p =>
                    {
                        /// Trường hợp đã mua hàng
                        if (p.ReservedQuantity > 0)
                        {
                            /// Trường hợp đã nhập kho hàng

                            /// Trường hợp chưa nhập kho hàng
                        }
                        /// Trường hợp chưa mua hàng
                        else
                        {

                        }
                    });
                }

                context.ProductionBOM.UpdateRange(productionBOMs);
                context.ReservationEntry.UpdateRange(reservationEntries);
                context.ItemStyle.UpdateRange(itemStyles);
                context.ItemStyleSyncMasters.UpdateRange(itemStyleSyncMasters);
                context.ItemStyleBarCode.UpdateRange(itemStyleBarcodes);
                context.SaveChanges();
            }
            catch(Exception ex)
            {
                logger.LogError("{@time} - Order detail bulk update event handler has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Order detail bulk update event handler has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
