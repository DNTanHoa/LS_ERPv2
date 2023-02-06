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
using Unit = LS_ERP.EntityFrameworkCore.Entities.Unit;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeleteUnitCommandHandler
        : IRequestHandler<DeleteUnitCommand, CommonCommandResult<Unit>>
    {
        private readonly ILogger<DeleteUnitCommandHandler> logger;
        private readonly IUnitRepository UnitRepository;
        private readonly SqlServerAppDbContext context;

        public DeleteUnitCommandHandler(ILogger<DeleteUnitCommandHandler> logger,
            IUnitRepository UnitRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.UnitRepository = UnitRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Unit>> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<Unit>();

            if (UnitRepository.IsExist(request.ID, out Unit willDeleteUnit))
            {
                result.Message = "Unit not exist";
                return result;
            }

            try
            {
                UnitRepository.Delete(willDeleteUnit);
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
