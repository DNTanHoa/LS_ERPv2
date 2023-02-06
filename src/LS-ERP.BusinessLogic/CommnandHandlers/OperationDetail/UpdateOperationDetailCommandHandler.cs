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
    public class UpdateOperationDetailCommandHandler
        : IRequestHandler<UpdateOperationDetailCommand, CommonCommandResultHasData<OperationDetail>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateOperationDetailCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateOperationDetailCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateOperationDetailCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<OperationDetail>> Handle(UpdateOperationDetailCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<OperationDetail>();
            logger.LogInformation("{@time} - Exceute update OperationDetail command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute update OperationDetail command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {
                var existOperationDetail = context.OperationDetail.FirstOrDefault(x => x.ID == request.ID);

                if(existOperationDetail != null)
                {
                    
                    mapper.Map(request, existOperationDetail);
                    var checkexitOperationDetail = context.OperationDetail.Where(x => x.MergeBlockLSStyle == existOperationDetail.MergeBlockLSStyle
                                                                        && x.Set == existOperationDetail.Set
                                                                        && x.FabricContrastName == existOperationDetail.FabricContrastName
                                                                        && x.IsPercentPrint == existOperationDetail.IsPercentPrint
                                                                        && x.OperationID == existOperationDetail.OperationID).FirstOrDefault();
                    if(checkexitOperationDetail==null)
                    {
                        existOperationDetail.OperationName = context.Operation.Where(x => x.ID == request.OperationID).FirstOrDefault().Name;
                        existOperationDetail.SetUpdateAudit(request.UserName);
                        context.OperationDetail.Update(existOperationDetail);
                        context.SaveChanges();
                        result.Success = true;
                        result.SetData(existOperationDetail);
                    }    
                    else
                    {
                        result.Success = false;
                        result.Message = "Đã tồn tại";
                        result.Data = null;
                    } 
                }
                else
                {
                    result.Message = "Can't not find to update";                
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update OperationDetail command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update OperationDetail command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        
    }
}
