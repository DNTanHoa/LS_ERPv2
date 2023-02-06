using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
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
    public class UpdateStatusFabricRequestCommandHandler
        : IRequestHandler<UpdateStatusFabricRequestCommand, UpdateStatusFabricRequestResult>
    {
        private readonly ILogger<UpdateStatusFabricRequestCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestRepository fabricRequestRepository;
        private readonly IMapper mapper;

        public UpdateStatusFabricRequestCommandHandler(ILogger<UpdateStatusFabricRequestCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator,
            IFabricRequestRepository fabricRequestRepository,
            IStatusRepository statusRepository,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
            this.fabricRequestRepository = fabricRequestRepository;
            this.mapper = mapper;
        }

        public async Task<UpdateStatusFabricRequestResult> Handle(UpdateStatusFabricRequestCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute update status fabric request command", DateTime.Now.ToString());
            var result = new UpdateStatusFabricRequestResult();

            var existFabricRequest = context.FabricRequest
                                            .Include(x => x.Details)
                                            .Include(x => x.Status).FirstOrDefault(x => x.ID == request.ID);

            try
            {
                if (existFabricRequest != null)
                {
                    existFabricRequest.SetUpdateAudit(request.UserName);
                    existFabricRequest.StatusID = request.StatusID?.ToUpper();
                    existFabricRequest.Remark = request.Remark;
                    //existFabricRequest.SetUpdateAudit(request.UserName);

                    context.FabricRequest.Update(existFabricRequest);

                    var fabricRequestLog = mapper.Map<FabricRequestLog>(existFabricRequest);

                    context.FabricRequestLog.Add(fabricRequestLog);

                    await context.SaveChangesAsync(cancellationToken);

                    result.IsSuccess = true;
                }
                else
                {
                    result.IsSuccess = false;

                    result.Message = "Can't not update because not found Fabric Request";
                }

            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
