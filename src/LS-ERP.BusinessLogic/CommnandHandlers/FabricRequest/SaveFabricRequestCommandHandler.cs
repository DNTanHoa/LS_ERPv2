using AutoMapper;
using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Process.Jobs;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class SaveFabricRequestCommandHandler
        : IRequestHandler<SaveFabricRequestCommand, SaveFabricRequestResult>
    {
        private readonly ILogger<SaveFabricRequestCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestRepository fabricRequestRepository;
        private readonly IMapper mapper;

        public SaveFabricRequestCommandHandler(ILogger<SaveFabricRequestCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator,
            IFabricRequestRepository fabricRequestRepository,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
            this.fabricRequestRepository = fabricRequestRepository;
            this.mapper = mapper;
        }

        public async Task<SaveFabricRequestResult> Handle(SaveFabricRequestCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute save fabric request command", DateTime.Now.ToString());
            var result = new SaveFabricRequestResult();

            var fabricRequest = mapper.Map<FabricRequest>(request);
            try
            {
                fabricRequest.SetCreateAudit(request.UserName);

                foreach (var detail in fabricRequest.Details)
                {
                    detail.SetCreateAudit(request.UserName);
                }

                context.FabricRequest.Add(fabricRequest);

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;

                result.Result = fabricRequest;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }
            finally
            {
                var jobId = BackgroundJob.Enqueue<InsertFabricRquestLogJob>(j => j.Execute(fabricRequest.ID));
            }

            return result;
        }
    }
}
