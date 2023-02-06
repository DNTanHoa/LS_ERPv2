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
    public class UpdateFabricRequestDetailCommandHandler
        : IRequestHandler<UpdateFabricRequestDetailCommand, UpdateFabricRequestDetailResult>
    {
        private readonly ILogger<UpdateFabricRequestDetailCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestDetailRepository fabricRequestDetailRepository;
        private readonly IMapper mapper;

        public UpdateFabricRequestDetailCommandHandler(ILogger<UpdateFabricRequestDetailCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator,
            IFabricRequestDetailRepository fabricRequestDetailRepository,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
            this.fabricRequestDetailRepository = fabricRequestDetailRepository;
            this.mapper = mapper;
        }

        public async Task<UpdateFabricRequestDetailResult> Handle(UpdateFabricRequestDetailCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute update fabric request command", DateTime.Now.ToString());
            var result = new UpdateFabricRequestDetailResult();

            var existFabricRequestDetail = context.FabricRequestDetail.FirstOrDefault(x => x.ID == request.ID);

            try
            {
                if (existFabricRequestDetail != null)
                {
                    var fabricRequestDetail = mapper.Map<FabricRequestDetail>(request);

                    fabricRequestDetail.SetUpdateAudit(request.UserName);

                    context.FabricRequestDetail.Update(fabricRequestDetail);

                    await context.SaveChangesAsync(cancellationToken);
                    result.IsSuccess = true;

                    result.Result = fabricRequestDetail;
                }
                else
                {
                    result.IsSuccess = false;

                    result.Message = "Can't not update because not found Fabric Request Detail";
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
