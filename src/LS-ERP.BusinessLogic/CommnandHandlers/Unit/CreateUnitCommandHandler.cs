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
    public class CreateUnitCommandHandler
        : IRequestHandler<CreateUnitCommand, CommonCommandResult<Unit>>
    {
        private readonly ILogger<CreateUnitCommandHandler> logger;
        private readonly IMapper mapper;
        private readonly IUnitRepository UnitRepository;
        private readonly SqlServerAppDbContext context;

        public CreateUnitCommandHandler(ILogger<CreateUnitCommandHandler> logger,
            IMapper mapper,
            IUnitRepository UnitRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.UnitRepository = UnitRepository;
            this.context = context;
        }

        public async Task<CommonCommandResult<Unit>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<Unit>();
            var unit = mapper.Map<Unit>(request);

            unit.SetCreateAudit(request.GetUser());

            try
            {
                unit = UnitRepository.Add(unit);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = unit;
            }
            catch (DbUpdateException ex)
            {
                if (UnitRepository.IsExist(request.ID))
                {
                    result.Message = "Unit with code " + request.ID + "has exist";
                }
                else
                {
                    result.Message = ex.InnerException.Message;
                }
            }
            return result;
        }
    }
}
