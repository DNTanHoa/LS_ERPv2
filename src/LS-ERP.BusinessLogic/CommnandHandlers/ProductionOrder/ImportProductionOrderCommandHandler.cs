using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ImportProductionOrderCommandHandler
        : IRequestHandler<ImportProductionOrderCommand, ImportProductionOrderResult>
    {
        private readonly ILogger<ImportProductionOrderCommandHandler> logger;

        public ImportProductionOrderCommandHandler(
            ILogger<ImportProductionOrderCommandHandler> logger)
        {
            this.logger = logger;
        }
        public Task<ImportProductionOrderResult> Handle(
            ImportProductionOrderCommand request, CancellationToken cancellationToken)
        {
            var result = new ImportProductionOrderResult();

            try
            {
                logger.LogInformation("{@time} - Exceute import production order command", 
                    DateTime.Now.ToString());
                LogHelper.Instance.Information("{@time} - Exceute import production order command",
                    DateTime.Now.ToString(), request.ToString());
                string fileName = request.File.FileName;

                if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
                {
                    result.Message = "Error saving file";
                    return Task.FromResult(result); 
                }


            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
                logger.LogInformation("{@time} - Execute import production order with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
                LogHelper.Instance.Information("{@time} - Execute import production order with request {@request} has error {@error}",
                    DateTime.Now.ToString(), request.ToString(), ex.InnerException.Message);
            }

            return Task.FromResult(result);
        }
    }
}
