using AutoMapper;
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
    public class ItemStylePullBomEventHandler : INotificationHandler<ItemStylePullBomEvent>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<ItemStylePullBomEventHandler> logger;
        private readonly IMapper mapper;

        public ItemStylePullBomEventHandler(SqlServerAppDbContext context,
            ILogger<ItemStylePullBomEventHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }
        public Task Handle(ItemStylePullBomEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                /// Danh sách material sync
                var newMaterialSyncs = new List<MaterialSync>();

                /// Lấy danh sách production BOM đã được pull
                var productionBOMs = context.ProductionBOM
                    .Where(x => notification.ItemStyleNumbers.Contains(x.ItemStyleNumber)).ToList();

                /// Lấy danh sách item style sync tương ứng
                var itemStyleSyncs = context.ItemStyleSyncMasters
                    .Where(x => notification.ItemStyleNumbers.Contains(x.ItemStyleNumber)).ToList();

                /// Lấy danh sách material sync cũ
                var materialSyncs = context.MaterialSyncs
                    .Where(x => itemStyleSyncs.Select(x => x.ID).Contains(x.ItemStyleSyncMasterID)).ToList();

                /// Cập nhật thông tin
                foreach(var itemStyleSync in itemStyleSyncs)
                {
                    var styleProductionBOMs = productionBOMs
                        .Where(x => x.ItemStyleNumber == itemStyleSync.ItemStyleNumber);
                    
                    if(styleProductionBOMs.Any())
                    {
                        foreach(var styleProductionBOM in styleProductionBOMs)
                        {
                            var materialSync = materialSyncs.FirstOrDefault(x => x.ItemStyleSyncMasterID == itemStyleSync.ID &&
                                                                                 x.ProductionBOMID == styleProductionBOM.ID);

                            if(materialSync != null)
                            {
                                mapper.Map(styleProductionBOM, materialSync);
                            }
                            else
                            {
                                materialSync = mapper.Map<MaterialSync>(styleProductionBOM);
                                newMaterialSyncs.Add(materialSync);
                            }

                            materialSync.ItemStyleSyncMasterID = itemStyleSync.ID;
                        }
                    }
                }

                context.MaterialSyncs.UpdateRange(materialSyncs);
                context.MaterialSyncs.AddRange(newMaterialSyncs);

                context.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                logger.LogError("ItemStyle pull bom event handler has error with message {@message}",
                        ex.InnerException?.Message);
                LogHelper.Instance.Error("ItemStyle pull bom event handler has error with message {@message}",
                    ex.InnerException?.Message);
            }

            return Task.CompletedTask;
        }
    }
}
