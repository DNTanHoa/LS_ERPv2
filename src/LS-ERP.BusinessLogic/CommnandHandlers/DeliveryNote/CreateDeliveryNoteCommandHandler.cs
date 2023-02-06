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
    public class CreateDeliveryNoteCommandHandler
        : IRequestHandler<CreateDeliveryNoteCommand, CommonCommandResultHasData<DeliveryNote>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateDeliveryNoteCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateDeliveryNoteCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateDeliveryNoteCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<DeliveryNote>> Handle(CreateDeliveryNoteCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<DeliveryNote>();
            logger.LogInformation("{@time} - Exceute create delivery note command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create delivery note command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            try
            {
                var deliveryNote = new DeliveryNote();
                deliveryNote = mapper.Map<DeliveryNote>(request);
                //create delivery code
                var today = DateTime.Now;
                var str = today.Year.ToString()+today.Month.ToString()+today.Day.ToString();
                bool exist = true;
                int idx = 0;
                string prifixCode = request.VendorID;
                var code = prifixCode;
                while(exist)
                {
                    idx++;
                    code = prifixCode + str + idx.ToString();
                    var exitDeliveryNote = context.DeliveryNote.Where(d => d.Code == code).FirstOrDefault();
                    if(exitDeliveryNote == null)
                    {
                        exist = false;
                    }    
                }
                var vender = context.Vendor.Where(x=>x.ID == request.VendorID).FirstOrDefault();
                deliveryNote.VendorName = vender?.Name;
                deliveryNote.Code = code;
                deliveryNote.Status = "NEW";
                deliveryNote.SendDate = DateTime.Now;
                deliveryNote.SetCreateAudit(request.UserName);
                context.DeliveryNote.Add(deliveryNote);
                context.SaveChanges();
                result.Success = true;
                result.SetData(deliveryNote);

            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute create delivery note command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute create delivery note command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }
            return Task.FromResult(result);
        }
       
    }
}
