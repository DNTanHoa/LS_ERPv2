using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Validator;
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
    public class UpdateUnitCommandHandler
        : IRequestHandler<UpdateUnitCommand, CommonCommandResult<Unit>>
    {
        private readonly ILogger<UpdateUnitCommandHandler> logger;
        private readonly IUnitRepository UnitRepository;
        private readonly IMapper mapper;
        private readonly SqlServerAppDbContext context;

        public UpdateUnitCommandHandler(ILogger<UpdateUnitCommandHandler> logger,
            IUnitRepository UnitRepository,
            IMapper mapper,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.UnitRepository = UnitRepository;
            this.mapper = mapper;
            this.context = context;
        }

        public async Task<CommonCommandResult<Unit>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Unit>();

            var existUnit = UnitRepository.GetUnit(request.ID);

            if (existUnit == null)
            {
                result.Message = "Not found Unit";
                return result;
            }

            mapper.Map(request, existUnit);

            existUnit.SetUpdateAudit(request.GetUser());

            try
            {
                UnitRepository.Update(existUnit);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = existUnit;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }
            return result;
        }
    }
}
