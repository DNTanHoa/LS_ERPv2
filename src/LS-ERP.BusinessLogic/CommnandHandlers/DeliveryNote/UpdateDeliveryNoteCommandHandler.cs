using AutoMapper;
using Common.Model;
using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;
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
    public class UpdateDeliveryNoteCommandHandler
        : IRequestHandler<UpdateDeliveryNoteCommand, CommonCommandResultHasData<DeliveryNote>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateDeliveryNoteCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateDeliveryNoteCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateDeliveryNoteCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<DeliveryNote>> Handle(UpdateDeliveryNoteCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<DeliveryNote>();
            logger.LogInformation("{@time} - Exceute update delivery note command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update delivery note command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existDeliveryNote = context.DeliveryNote.FirstOrDefault(x => x.ID == request.ID);

                if(existDeliveryNote != null)
                {
                    //mapper.Map(request, existDeliveryNote);
                    existDeliveryNote.Remark = request.Remark;
                    existDeliveryNote.IsSend = request.IsSend;
                    if(existDeliveryNote.IsSend)
                    {
                        existDeliveryNote.Status = "SEND";
                        var deliveryNoteDetails = context.DeliveryNoteDetail.Where(d=>d.DeliveryNoteID == request.ID).ToList();
                        foreach(var deliveryNoteDetail in deliveryNoteDetails)
                        {
                            deliveryNoteDetail.IsSend = true;
                            deliveryNoteDetail.Status = "SEND";
                            deliveryNoteDetail.SendDate = DateTime.Now;
                            deliveryNoteDetail.SetUpdateAudit(request.UserName);
                        }    
                        context.DeliveryNoteDetail.UpdateRange(deliveryNoteDetails);
                        context.SaveChanges();
                    }
                    else
                    {
                        existDeliveryNote.Status = "NEW";
                        var deliveryNoteDetails = context.DeliveryNoteDetail.Where(d => d.DeliveryNoteID == request.ID).ToList();
                        foreach (var deliveryNoteDetail in deliveryNoteDetails)
                        {
                            deliveryNoteDetail.IsSend = false;
                            deliveryNoteDetail.Status = "NEW";
                            deliveryNoteDetail.SendDate = DateTime.Now;
                            deliveryNoteDetail.SetUpdateAudit(request.UserName);
                        }
                        context.DeliveryNoteDetail.UpdateRange(deliveryNoteDetails);
                        context.SaveChanges();
                    }    
                    existDeliveryNote.SetUpdateAudit(request.UserName);
                    context.DeliveryNote.Update(existDeliveryNote);
                    context.SaveChanges();
                }
                else
                {
                    result.Message = "Can't not find to update";
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update delivery note command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update delivery note command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        
        
        
    }
}
