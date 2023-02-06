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
    public class SaveFabricRequestDetailCommandHandler
        : IRequestHandler<SaveFabricRequestDetailCommand, SaveFabricRequestDetailResult>
    {
        private readonly ILogger<SaveFabricRequestDetailCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestDetailRepository fabricRequestDetailRepository;
        private readonly IMapper mapper;

        public SaveFabricRequestDetailCommandHandler(ILogger<SaveFabricRequestDetailCommandHandler> logger,
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

        public async Task<SaveFabricRequestDetailResult> Handle(SaveFabricRequestDetailCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute save fabric request detail command", DateTime.Now.ToString());
            var result = new SaveFabricRequestDetailResult();

            var fabricRequestDetail = mapper.Map<FabricRequestDetail>(request);
            try
            {
                fabricRequestDetail.SetCreateAudit(request.UserName);
                context.FabricRequestDetail.Add(fabricRequestDetail);

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;

                result.Result = fabricRequestDetail;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
