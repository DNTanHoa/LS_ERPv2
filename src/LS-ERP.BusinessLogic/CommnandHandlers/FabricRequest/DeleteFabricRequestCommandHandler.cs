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
    public class DeleteFabricRequestCommandHandler
         : IRequestHandler<DeleteFabricRequestCommand, DeleteFabricRequestResult>
    {
        private readonly ILogger<DeleteFabricRequestCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestRepository fabricRequestRepository;
        private readonly IMapper mapper;

        public DeleteFabricRequestCommandHandler(ILogger<DeleteFabricRequestCommandHandler> logger,
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

        public async Task<DeleteFabricRequestResult> Handle(DeleteFabricRequestCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute delete fabric request command", DateTime.Now.ToString());
            var result = new DeleteFabricRequestResult();

            try
            {
                var existsFabricRequest = fabricRequestRepository.GetFabricRequest(request.ID);

                if (existsFabricRequest != null && existsFabricRequest.StatusID != "A")
                {
                    context.FabricRequestDetail.RemoveRange(existsFabricRequest.Details);

                    existsFabricRequest.Status = null;

                    context.FabricRequest.Remove(existsFabricRequest);

                    await context.SaveChangesAsync(cancellationToken);
                    result.IsSuccess = true;
                }
                else
                {
                    if (existsFabricRequest != null && existsFabricRequest.StatusID == "A")
                    {
                        result.IsSuccess = false;
                        result.Message = "Can't delete because fabric request has been approved";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "Can't delete because not found item";
                    }
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
