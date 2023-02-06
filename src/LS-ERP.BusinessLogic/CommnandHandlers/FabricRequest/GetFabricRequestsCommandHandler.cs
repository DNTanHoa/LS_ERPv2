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
    public class GetFabricRequestsCommandHandler
         : IRequestHandler<GetFabricRequestsCommand, GetFabricRequestsResult>
    {
        private readonly ILogger<GetFabricRequestsCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;
        private readonly IFabricRequestRepository fabricRequestRepository;
        private readonly IMapper mapper;

        public GetFabricRequestsCommandHandler(ILogger<GetFabricRequestsCommandHandler> logger,
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

        public async Task<GetFabricRequestsResult> Handle(GetFabricRequestsCommand request, CancellationToken cancellationToken)
        {
            var result = new GetFabricRequestsResult();

            try
            {
                var fabricRequests = fabricRequestRepository.GetFabricRequests(request.CustomerID, request.CompanyCode, request.FromDate, request.ToDate);

                if (fabricRequests != null)
                {
                    result.IsSuccess = true;
                    result.Result = fabricRequests.ToList();
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "Not found Fabric Request";
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
