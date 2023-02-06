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
    public class DeleteFabricRequestDetailCommandHandler
         : IRequestHandler<DeleteFabricRequestDetailCommand, DeleteFabricRequestDetailResult>
    {
        private readonly ILogger<DeleteFabricRequestDetailCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestDetailRepository fabricRequestDetailRepository;
        private readonly IMapper mapper;

        public DeleteFabricRequestDetailCommandHandler(ILogger<DeleteFabricRequestDetailCommandHandler> logger,
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

        public async Task<DeleteFabricRequestDetailResult> Handle(DeleteFabricRequestDetailCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute delete fabric request detail command", DateTime.Now.ToString());
            var result = new DeleteFabricRequestDetailResult();

            try
            {

                var fabricRequestDetail = fabricRequestDetailRepository.GetFabricRequestDetail(request.ID);

                context.FabricRequestDetail.Remove(fabricRequestDetail);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;

            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
