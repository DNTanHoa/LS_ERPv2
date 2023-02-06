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
    public class GetFabricRequestDetailLogCommandHandler :
        IRequestHandler<GetFabricRequestDetailLogsCommand, GetFabricRequestDetailLogsResult>
    {
        private readonly ILogger<GetFabricRequestDetailLogCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestDetailLogRepository fabricRequestDetailLogRepository;
        private readonly IMapper mapper;

        public GetFabricRequestDetailLogCommandHandler(ILogger<GetFabricRequestDetailLogCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator,
            IFabricRequestDetailLogRepository fabricRequestDetailLogRepository,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
            this.fabricRequestDetailLogRepository = fabricRequestDetailLogRepository;
            this.mapper = mapper;
        }
        public async Task<GetFabricRequestDetailLogsResult> Handle(GetFabricRequestDetailLogsCommand request, CancellationToken cancellationToken)
        {
            var result = new GetFabricRequestDetailLogsResult();

            try
            {
                var fabricRequestDetailLogs = fabricRequestDetailLogRepository.GetFabricRequestDetailLogs(request.ID);

                if (fabricRequestDetailLogs != null)
                {
                    result.IsSuccess = true;
                    result.Result = fabricRequestDetailLogs.ToList();
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
