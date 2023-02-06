using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
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
    public class UpdateIssuedCommandHandler
        : IRequestHandler<UpdateIssuedCommand, CommonCommandResult<Issued>>
    {
        private readonly ILogger<UpdateIssuedCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMapper mapper;

        public UpdateIssuedCommandHandler(ILogger<UpdateIssuedCommandHandler> logger,
            SqlServerAppDbContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<CommonCommandResult<Issued>> Handle(UpdateIssuedCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<Issued>();

            logger.LogInformation("{@time} - Execute update issued command with request {@request}",
                DateTime.Now.ToString(), request.ToString());
            LogHelper.Instance.Information("{@time} - Execute update issued command with request {@request}",
                DateTime.Now.ToString(), request.ToString());

            try
            {

                var issued = context.Issued.FirstOrDefault(x => x.Number == request.Number);

                if (issued != null)
                {

                    mapper.Map(request, issued);
                    issued.SetUpdateAudit(request.Username);
                    context.Issued.Update(issued);

                    await context.SaveChangesAsync(cancellationToken);
                    result.IsSuccess = true;
                }
                else
                {
                    result.Message = "Not found issued to update";
                }
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
