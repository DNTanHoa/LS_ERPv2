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
    public class UpdateDivisionCommandHandler
        : IRequestHandler<UpdateDivisionCommand, CommonCommandResult<Division>>
    {
        private readonly ILogger<UpdateDivisionCommandHandler> logger;
        private readonly IDivisionRepository divisionRepository;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public UpdateDivisionCommandHandler(ILogger<UpdateDivisionCommandHandler> logger,
            IDivisionRepository divisionRepository,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.divisionRepository = divisionRepository;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CommonCommandResult<Division>> Handle(UpdateDivisionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Division>();

            var existDivision = divisionRepository.GetDivision(request.ID);

            if(existDivision == null)
            {
                result.Message = "Not found division";
                return result;
            }
            
            mapper.Map(request, existDivision);
            existDivision.SetUpdateAudit(request.GetUser());

            try
            {
                divisionRepository.Update(existDivision);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = existDivision;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
