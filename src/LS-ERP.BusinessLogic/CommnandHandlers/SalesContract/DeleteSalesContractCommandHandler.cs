using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class DeleteSalesContractCommandHandler
        : IRequestHandler<DeleteSalesContractCommand, DeleteSalesContractResult>
    {
        private readonly ILogger<DeleteSalesContractCommandHandler> logger;
        private readonly ISalesContractRepository salesContractRepository;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public DeleteSalesContractCommandHandler(ILogger<DeleteSalesContractCommandHandler> logger,
            ISalesContractRepository salesContractRepository,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.salesContractRepository = salesContractRepository;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<DeleteSalesContractResult> Handle(DeleteSalesContractCommand request, 
            CancellationToken cancellationToken)
        {
            var result = new DeleteSalesContractResult();

            if (!salesContractRepository.IsExist(request.ID))
            {
                result.Message = "Not found contract to delete";
                return result;
            }

            var willDeleteContract = salesContractRepository.GetSalesContract(request.ID);
            context.SalesContractDetail.RemoveRange(willDeleteContract.ContractDetails);
            context.SalesContract.Remove(willDeleteContract);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException?.Message;
            }

            return result;
        }
    }
}
