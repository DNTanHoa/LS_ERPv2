using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class DeleteForecastEntryCommandHandler
        : IRequestHandler<DeleteForecastEnrtryCommand, CommonCommandResult>
    {
        private readonly SqlServerAppDbContext context;
        private readonly ILogger<DeleteForecastEntryCommandHandler> logger;
        private readonly ForecastEntryValidator validator;

        public DeleteForecastEntryCommandHandler(SqlServerAppDbContext context,
            ILogger<DeleteForecastEntryCommandHandler> logger,
            ForecastEntryValidator validator)
        {
            this.context = context;
            this.logger = logger;
            this.validator = validator;
        }
        public async Task<CommonCommandResult> Handle(DeleteForecastEnrtryCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult();

            try
            {
                var forcastEntry = context.ForecastEntry
                    .Include(x => x.ForecastOveralls)
                    .ThenInclude(x => x.ForecastMaterials)
                    .FirstOrDefault(x => x.ID == request.ForecastEntryID);

                if(forcastEntry != null)
                {

                    if (validator.CanDelete(forcastEntry))
                    {
                        context.ForecastMaterial
                            .RemoveRange(forcastEntry.ForecastOveralls
                                .SelectMany(x => x.ForecastMaterials));
                        context.ForecastDetail
                            .RemoveRange(forcastEntry.ForecastOveralls
                                .SelectMany(x => x.ForecastDetails));
                        context.ForecastMaterial
                            .RemoveRange(forcastEntry.ForecastOveralls
                                .SelectMany(x => x.ForecastMaterials));

                        context.SaveChanges();
                        result.SetResult(true, string.Empty);
                    }
                    else
                    {
                        result.Message = "Invalid. Can't delete this forecast";
                    }
                }
                else
                {
                    result.Message = "Not found to delete";
                }
            }
            catch(DbUpdateException ex)
            {
                result.Message = ex.Message;
                logger.LogError("{@time} - Purchase order created event handler for request {@request} " +
                    "has error with message {@mesage}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(request), ex.InnerException?.Message);
                LogHelper.Instance.Error("{@time} - Purchase order created event handler for request {@request} " +
                    "has error with message {@mesage}",
                    DateTime.Now.ToString(), JsonConvert.SerializeObject(request), ex.InnerException?.Message);
            }

            return result;
        }
    }
}
