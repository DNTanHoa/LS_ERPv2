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
    public class DeleteDivisionCommandHandler :
        IRequestHandler<DeleteDivisionCommand, CommonCommandResult<Division>>
    {
        private readonly ILogger<DeleteDivisionCommandHandler> logger;
        private readonly IDivisionRepository divisionRepository;
        private readonly SqlServerAppDbContext context;

        public DeleteDivisionCommandHandler(ILogger<DeleteDivisionCommandHandler> logger,
            IDivisionRepository divisionRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.divisionRepository = divisionRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Division>> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Division>();

            var existDivision = divisionRepository.GetDivision(request.ID);

            if (existDivision == null)
            {
                result.Message = "Not found division";
                return result;
            }

            try
            {
                divisionRepository.Delete(existDivision);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
