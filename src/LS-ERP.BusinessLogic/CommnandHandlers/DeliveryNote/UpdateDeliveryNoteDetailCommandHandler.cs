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
    public class UpdateDeliveryNoteDetailCommandHandler
        : IRequestHandler<UpdateDeliveryNoteDetailCommand, CommonCommandResultHasData<DeliveryNoteDetail>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<UpdateDeliveryNoteDetailCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateDeliveryNoteDetailCommandHandler(SqlServerAppDbContext context,
            ILogger<UpdateDeliveryNoteDetailCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<DeliveryNoteDetail>> Handle(UpdateDeliveryNoteDetailCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<DeliveryNoteDetail>();           
          
            logger.LogInformation("{@time} - Exceute delete delivery note detail command with request {@request}",
            DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute delete delivery note detail command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            try
            {
                
                foreach (var ID in request.Ids)
                {
                    var existDeliveryNoteDetail = context.DeliveryNoteDetail.FirstOrDefault(x => x.ID == ID && x.IsSend == false);
                    if (existDeliveryNoteDetail != null)
                    {
                        existDeliveryNoteDetail.IsDeleted = true;  
                        existDeliveryNoteDetail.SetUpdateAudit(request.UserName);
                        context.DeliveryNoteDetail.Update(existDeliveryNoteDetail);
                        context.SaveChanges();
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "Can't not find to update";
                    }
                  
                }

            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update cutting card location command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update cutting card location command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }
           
            return Task.FromResult(result);
        }
        
    }
}
