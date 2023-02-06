using Common.Model;
using LS_ERP.BusinessLogic.Commands;
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

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeleteDeliveryNoteCommandHandler
        : IRequestHandler<DeleteDeliveryNoteCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<DeleteDeliveryNoteCommandHandler> logger;

        public DeleteDeliveryNoteCommandHandler(SqlServerAppDbContext context,
            ILogger<DeleteDeliveryNoteCommandHandler> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public Task<CommonCommandResult> Handle(DeleteDeliveryNoteCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            logger.LogInformation("{@time} - Exceute delete delivery note command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute delete delivery note command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existDeliveryNote = context.DeliveryNote.FirstOrDefault(x => x.ID == request.ID);

                if (existDeliveryNote != null)
                {

                    //context.DeliveryNote.Remove(existDeliveryNote);
                    existDeliveryNote.IsDeleted = true; 
                    var deliveryNoteDetails = context.DeliveryNoteDetail.Where(x=>x.DeliveryNoteID == existDeliveryNote.ID).ToList();
                    foreach(var deliveryNoteDetail in deliveryNoteDetails)
                    {
                        deliveryNoteDetail.IsDeleted = true;
                        deliveryNoteDetail.SetUpdateAudit(request.UserName);
                    }
                    existDeliveryNote.SetUpdateAudit(request.UserName);
                    context.DeliveryNote.Update(existDeliveryNote);
                    context.DeliveryNoteDetail.UpdateRange(deliveryNoteDetails);
                    context.SaveChanges();

                    result.Success = true;
                }
                else
                {
                    result.Message = "Can't not find to delete";
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute delete delivery note command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute delete delivery note command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        
    }
}
