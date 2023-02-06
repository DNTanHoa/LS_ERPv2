﻿using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class UpdateCAC_CHD_EHDCommandHandler : IRequestHandler<UpdateCAC_CHD_EHDCommand, UpdateCAC_CHD_EHDResult>
    {
        private readonly ILogger<UpdateCAC_CHD_EHDCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly ISalesOrderRepository salesOrderRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IMediator mediator;

        public UpdateCAC_CHD_EHDCommandHandler(ILogger<UpdateCAC_CHD_EHDCommandHandler> logger,
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

        public async Task<UpdateCAC_CHD_EHDResult> Handle(UpdateCAC_CHD_EHDCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Execute update CAC/CHD/EHD for sales order command", DateTime.Now.ToString());
            var result = new UpdateCAC_CHD_EHDResult();

            if (!FileHelpers.SaveFile(request.UpdateFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var salesOrders = salesOrderRepository
                .GetSalesOrders(request.CustomerID);

            var updateResult = UpdateCAC_CHD_EHDProcess
                .UpdateMultiSalesOrder(salesOrders,
                     request.UserName, fullPath,
                     out List<ItemStyle> itemStyles,
                     out string errorMessage);

            if (!updateResult)
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                var updateItemSyncMasters = new List<ItemStyleSyncMaster>();

                var itemSyncMasters = context.ItemStyleSyncMasters
                        .Where(x => itemStyles.Select(x => x.Number).Contains(x.ItemStyleNumber)).ToList();

                itemStyles.ForEach(i =>
                {
                    var itemStyleSyncMasters = itemSyncMasters
                            .Where(x => x.ItemStyleNumber == i.Number).ToList();

                    itemStyleSyncMasters.ForEach(x =>
                    {
                        x.DeliveryPlace = i.DeliveryPlace;
                        x.ContractualSupplierHandover = i.ContractDate;
                        x.ProductionSkedDeliveryDate = i.ProductionSkedDeliveryDate;
                    });

                    updateItemSyncMasters.AddRange(itemStyleSyncMasters);
                });
                
                context.ItemStyle.UpdateRange(itemStyles);

                context.ItemStyleSyncMasters.UpdateRange(updateItemSyncMasters);

                context.SaveChanges();

                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                LogHelper.Instance.Error("{@time} - Execute update CAC/CHD/EHD for sales order command with message {@message}",
                    DateTime.Now.ToString(), ex.InnerException?.Message);
            }

            return result;
        }

    }
}
