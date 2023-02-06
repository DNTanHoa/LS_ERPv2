using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
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
    public class CreatePartRevisionCommandHandler
        : IRequestHandler<CreatePartRevisionCommand, CommonCommandResult<PartRevision>>
    {
        private readonly ILogger<CreatePartRevisionCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public CreatePartRevisionCommandHandler(ILogger<CreatePartRevisionCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CommonCommandResult<PartRevision>> Handle(CreatePartRevisionCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new CommonCommandResult<PartRevision>();
            var partRevison = mapper.Map<PartRevision>(request);

            partRevison.SetCreateAudit(request.GetUser());

            try
            {
                context.Item.AddRange(request.Items);
                context.PartRevision.Add(partRevison);
                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
                result.Result = partRevison;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }
            return result;
        }
    }
}
