using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ImportDailyTargetCommandHandler
     : IRequestHandler<ImportDailyTargetCommand, ImportDailyTargetResult>
    {
        private readonly ILogger<ImportDailyTargetCommandHandler> logger;
        private readonly SqlServerAppDbContext context;

        public ImportDailyTargetCommandHandler(ILogger<ImportDailyTargetCommandHandler> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        public Task<ImportDailyTargetResult> Handle(ImportDailyTargetCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import daily target command", DateTime.Now.ToString());

            var result = new ImportDailyTargetResult();

            if (!FileHelpers.SaveFile(request.ImportFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return Task.FromResult(result);
            }
     
            bool importResult;

            importResult = ImportDailyTargetProcess.Import(fullPath, request.Username, request.ImportFile.FileName,
                                       out List<DailyTarget> dailyTargets,
                                       out string errorMessage);

            if (!importResult)
            {
                result.Message = errorMessage;
            }
            else
            {
                result.Data = dailyTargets;
                result.IsSuccess = true;
            }

            return Task.FromResult(result);
        }
    }
}
