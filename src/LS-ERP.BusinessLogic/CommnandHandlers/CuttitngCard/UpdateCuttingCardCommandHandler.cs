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
    public class UpdateCuttingCardCommandHandler
        : IRequestHandler<UpdateCuttingCardCommand, CommonCommandResultHasData<CuttingCard>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateCuttingCardCommandHandler> logger;
        private readonly IMapper mapper;

        public UpdateCuttingCardCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateCuttingCardCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<CuttingCard>> Handle(UpdateCuttingCardCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<CuttingCard>();
            if(string.IsNullOrEmpty(request.CurrentOperation))
            {
                logger.LogInformation("{@time} - Exceute update cutting card command with request {@request}",
               DateTime.Now.ToString(), request.ToString());
                LogHelper.Instance.Information("{@time} - Exceute update cutting card command with request {@request}",
                    DateTime.Now.ToString(), request.ToString());
            }    
            else if(request.CurrentOperation == "SUPPERMARKET")
            {
                logger.LogInformation("{@time} - Exceute update cutting card location command with request {@request}",
              DateTime.Now.ToString(), request.ToString());
                LogHelper.Instance.Information("{@time} - Exceute update cutting card location command with request {@request}",
                    DateTime.Now.ToString(), request.ToString());
                return Task.FromResult(result);
            }    


            try
            {
                var existCuttingCard = context.CuttingCard.FirstOrDefault(x => x.ID == request.ID && x.Operation == request.Operation);

                if(existCuttingCard != null)
                {
                    
                    mapper.Map(request, existCuttingCard);
                      
                    existCuttingCard.SetUpdateAudit(request.UserName);
                    context.CuttingCard.Update(existCuttingCard);
                    context.SaveChanges();
                    // Call Job to create,update CuttingCard with operation detail
                    if(existCuttingCard.CardType == "MASTER")
                    {
                        var jobId = BackgroundJob.Enqueue<CreateCuttingCardOperationDetailJob>(j => j.Execute(existCuttingCard, request.UserName));
                    }  
                    result.Success = true;
                    result.SetData(existCuttingCard);
                }
                else
                {
                    result.Message = "Can't not find to update";                    
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute update cutting card command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute update cutting card command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
        
    }
}
