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
    public class GetFabricRequestCommandHandler
        : IRequestHandler<GetFabricRequestCommand, GetFabricRequestResult>
    {
        private readonly ILogger<GetFabricRequestCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IFabricRequestRepository fabricRequestRepository;
        private readonly IMapper mapper;

        public GetFabricRequestCommandHandler(ILogger<GetFabricRequestCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator,
            IFabricRequestRepository fabricRequestRepository,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.fabricRequestRepository = fabricRequestRepository;
            this.mapper = mapper;
        }

        public async Task<GetFabricRequestResult> Handle(GetFabricRequestCommand request, CancellationToken cancellationToken)
        {
            var result = new GetFabricRequestResult();

            try
            {
                var fabricRequest = fabricRequestRepository.GetFabricRequest(request.ID);

                if (fabricRequest != null)
                {
                    result.IsSuccess = true;
                    result.Result = fabricRequest;
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
