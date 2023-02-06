using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
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
    public class ImportPartPriceCommandHandler : IRequestHandler<ImportPartPriceCommand, ImportPartPriceResult>
    {
        private readonly ILogger<ImportPartPriceCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;

        public ImportPartPriceCommandHandler(ILogger<ImportPartPriceCommandHandler> logger,
           SqlServerAppDbContext context,
           IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
        }

        public async Task<ImportPartPriceResult> Handle(ImportPartPriceCommand request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Execute import part price command", DateTime.Now.ToString());
            var result = new ImportPartPriceResult();

            if (!FileHelpers.SaveFile(request.UpdateFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            try
            {
                var partPrices = ImportPartPriceProcess
                    .Import(fullPath, request.CustomerID, request.UserName, out string errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    result.Message = errorMessage;
                    return result;
                }
                else
                {
                    context.PartPrice.AddRange(partPrices);

                    context.SaveChanges();
                    result.IsSuccess = true;
                }
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                LogHelper.Instance.Error("{@time} - Execute import part price command with message {@message}",
                    DateTime.Now.ToString(), ex.InnerException?.Message);
            }

            return result;
        }
    }
}
