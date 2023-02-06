using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class CreateDivisionCommandHandler :
        IRequestHandler<CreateDivisionCommand, CommonCommandResult<Division>>
    {
        private readonly ILogger<CreateDivisionCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly IDivisionRepository divisionRepository;
        private readonly SqlServerAppDbContext context;

        public CreateDivisionCommandHandler(ILogger<CreateDivisionCommandHandler> logger,
            IMapper mapper,
            IDivisionRepository divisionRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.divisionRepository = divisionRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Division>> Handle(CreateDivisionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Division>();
            var division = mapper.Map<Division>(request);
            division.SetCreateAudit(request.GetUser());

            try
            {
                division = divisionRepository.Add(division);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = division;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
