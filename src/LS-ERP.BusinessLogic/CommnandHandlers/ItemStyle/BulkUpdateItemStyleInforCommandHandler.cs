using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class BulkUpdateItemStyleInforCommandHandler
        : IRequestHandler<BulkUpdateItemStyleInforCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;

        public BulkUpdateItemStyleInforCommandHandler(SqlServerAppDbContext context)
        {
            this.context = context;
        }
        public Task<CommonCommandResult> Handle(BulkUpdateItemStyleInforCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                var styles = request.Data.Select(x => x.LSStyle);
                
                ///Get itemstyle infor
                var itemStyles = context.ItemStyle
                    .Where(x => styles.Contains(x.LSStyle)).ToList();
                var itemStyleNumbers = itemStyles.Select(x => x.Number);

                ///Get itemstyle sync master
                var itemStyleSyncMasters = context.ItemStyleSyncMasters
                    .Where(x => itemStyleNumbers.Contains(x.ItemStyleNumber))
                    .ToList();

                var itemStylesSyncActions = new List<ItemStyleSyncAction>();

                foreach (var itemStyleSyncMaster in itemStyleSyncMasters)
                {
                    var updateInfor = request.Data
                        .FirstOrDefault(x => x.LSStyle == itemStyleSyncMaster.LSStyle);
                    
                    if(updateInfor != null)
                    {
                        itemStyleSyncMaster.IssuedDateBU = updateInfor.IssuedDate;
                        itemStyleSyncMaster.AccessoriesDate = updateInfor.AccessoriesDate;
                        itemStyleSyncMaster.FabricDate = updateInfor.FabricDate;
                        itemStyleSyncMaster.ProductionSkedDeliveryDate = 
                            updateInfor.ProductionSketDeliveryDate;
                        itemStyleSyncMaster.Remark = updateInfor.Remark;

                        itemStyleSyncMaster.SetUpdateAudit(request.UserName);

                        /// Create action
                        var action = new ItemStyleSyncAction()
                        {
                            ItemStyleSyncMasterID = itemStyleSyncMaster.ID,
                            Action = "Update infor",
                            ActionDate = DateTime.Now
                        };

                        action.SetCreateAudit(request.UserName);
                        itemStylesSyncActions.Add(action);
                    }
                }

                context.ItemStyleSyncMasters.UpdateRange(itemStyleSyncMasters);
                context.ItemStyleSyncActions.AddRange(itemStylesSyncActions);
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
