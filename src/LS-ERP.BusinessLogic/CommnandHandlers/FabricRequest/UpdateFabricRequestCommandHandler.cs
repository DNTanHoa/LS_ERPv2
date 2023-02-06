using AutoMapper;
using Hangfire;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Process;
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
    public class UpdateFabricRequestCommandHandler
         : IRequestHandler<UpdateFabricRequestCommand, UpdateFabricRequestResult>
    {
        private readonly ILogger<UpdateFabricRequestCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestRepository fabricRequestRepository;
        private readonly IMapper mapper;

        public UpdateFabricRequestCommandHandler(ILogger<UpdateFabricRequestCommandHandler> logger,
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

        public async Task<UpdateFabricRequestResult> Handle(UpdateFabricRequestCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute update fabric request command", DateTime.Now.ToString());
            var result = new UpdateFabricRequestResult();

            var existFabricRequest = fabricRequestRepository.GetFabricRequest(request.ID);

            try
            {
                if (existFabricRequest != null && existFabricRequest.StatusID != "A")
                {
                    var newFabricRequest = mapper.Map<FabricRequest>(request);
                    newFabricRequest.CreatedAt = existFabricRequest.CreatedAt;
                    newFabricRequest.CreatedBy = existFabricRequest.CreatedBy;

                    FabricRequestProcessor.Update(request.UserName, newFabricRequest, existFabricRequest,
                                                  out List<FabricRequestDetail> newRequestDetails,
                                                  out List<FabricRequestDetail> updateRequestDetails,
                                                  out List<FabricRequestDetail> deleteRequestDetails);

                    context.FabricRequestDetail.AddRange(newRequestDetails);
                    context.FabricRequestDetail.UpdateRange(updateRequestDetails);
                    context.FabricRequestDetail.RemoveRange(deleteRequestDetails);

                    existFabricRequest = mapper.Map<FabricRequest>(newFabricRequest);

                    existFabricRequest.SetUpdateAudit(request.UserName);
                    existFabricRequest.CreatedAt = newFabricRequest.CreatedAt;
                    existFabricRequest.CreatedBy = newFabricRequest.CreatedBy;

                    context.FabricRequest.Update(existFabricRequest);

                    await context.SaveChangesAsync(cancellationToken);
                    result.IsSuccess = true;

                    result.Result = existFabricRequest;
                }
                else
                {
                    result.IsSuccess = false;

                    result.Message = "Can't not update because not found Fabric Request or it has been Approved";
                }

            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }
            finally
            {
                var jobId = BackgroundJob.Enqueue<InsertFabricRquestLogJob>(j => j.Execute(request.ID));
            }

            return result;
        }
    }
}
