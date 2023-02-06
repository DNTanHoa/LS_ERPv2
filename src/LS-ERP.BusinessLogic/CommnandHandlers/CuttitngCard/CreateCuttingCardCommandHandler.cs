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
    public class CreateCuttingCardCommandHandler
        : IRequestHandler<CreateCuttingCardCommand, CommonCommandResultHasData<CuttingCard>>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<CreateCuttingCardCommandHandler> logger;
        private readonly IMapper mapper;

        public CreateCuttingCardCommandHandler(SqlServerAppDbContext context,
            ILogger<CreateCuttingCardCommandHandler> logger,
            IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public Task<CommonCommandResultHasData<CuttingCard>> Handle(CreateCuttingCardCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<CuttingCard>();
            logger.LogInformation("{@time} - Exceute create cutting card command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Exceute create cutting card command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            var CuttingCard = new CuttingCard();
            CuttingCard = mapper.Map<CuttingCard>(request);
            try
            {
                CuttingCard.SetCreateAudit(request.UserName);
                context.CuttingCard.Add(CuttingCard);
                context.SaveChanges();                 
                result.Success = true;
                result.SetData(CuttingCard);
               
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Exceute create cutting output command with request {@request} has error {@error}",
                DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Exceute create cutting output command with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }
            return Task.FromResult(result);
        }
       
    }
}
