using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
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
    public class GetFabricRequestLogCommandHandler :
        IRequestHandler<GetFabricRequestLogsCommand, GetFabricRequestLogsResult>
    {
        private readonly ILogger<GetFabricRequestsCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestLogRepository fabricRequestLogRepository;
        private readonly IMapper mapper;

        public GetFabricRequestLogCommandHandler(ILogger<GetFabricRequestsCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator,
            IFabricRequestLogRepository fabricRequestLogRepository,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
            this.fabricRequestLogRepository = fabricRequestLogRepository;
            this.mapper = mapper;
        }

        public async Task<GetFabricRequestLogsResult> Handle(GetFabricRequestLogsCommand request, CancellationToken cancellationToken)
        {
            var result = new GetFabricRequestLogsResult();

            try
            {
                var fabricRequestLogs = fabricRequestLogRepository.GetFabricRequestLogs(request.ID);

                if (fabricRequestLogs != null)
                {
                    result.IsSuccess = true;
                    result.Result = fabricRequestLogs.ToList();
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Not found Fabric Request Log";
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
