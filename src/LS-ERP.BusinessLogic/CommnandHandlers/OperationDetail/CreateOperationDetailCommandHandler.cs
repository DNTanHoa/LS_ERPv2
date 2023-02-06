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
    public class CreateOperationDetailCommandHandler
        : IRequestHandler<CreateOperationDetailCommand, CommonCommandResultHasData<OperationDetail>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateOperationDetailCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateOperationDetailCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateOperationDetailCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<OperationDetail>> Handle(CreateOperationDetailCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<OperationDetail>();
            logger.LogInformation("{@time} - Exceute create OperationDetail command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute OperationDetail command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            var OperationDetail = new OperationDetail();
            OperationDetail = mapper.Map<OperationDetail>(request);
            try
            {
                var exitOperationDetail = context.OperationDetail.Where(x => x.MergeBlockLSStyle == request.MergeBlockLSStyle
                                                                        && x.Set == request.Set
                                                                        && x.FabricContrastName == request.FabricContrastName
                                                                        && x.OperationID == request.OperationID).FirstOrDefault();
                if(exitOperationDetail == null)
                {
                    OperationDetail.OperationName = context.Operation.Where(x => x.ID == request.OperationID).FirstOrDefault().Name;
                    OperationDetail.SetCreateAudit(request.UserName);
                    context.OperationDetail.Add(OperationDetail);
                    context.SaveChanges();
                    result.Success = true;
                    result.SetData(OperationDetail);
                }    
                else
                {
                    result.Success = false;
                    result.Message = "Đã tồn tại";
                    result.Data = null;
                }    
               
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute create OperationDetail command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute create OperationDetail command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }
            return Task.FromResult(result);
        }
       
    }
}
