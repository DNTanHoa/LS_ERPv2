using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
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
    public class ImportForecastGroupCommandHandler
        : IRequestHandler<ImportForecastGroupCommand, CommonCommandResult<List<ForecastOverall>>>
    {
        private readonly ILogger<ImportForecastGroupCommandHandler> logger;
        private readonly ISizeRepository sizeRepository;

        public ImportForecastGroupCommandHandler(ILogger<ImportForecastGroupCommandHandler> logger,
            ISizeRepository sizeRepository)
        {
            this.logger = logger;
            this.sizeRepository = sizeRepository;
        }

        public async Task<CommonCommandResult<List<ForecastOverall>>> Handle(
            ImportForecastGroupCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<List<ForecastOverall>>();

            logger.LogInformation("{@time} - Exceute import forecast group command for user {@user}", 
                DateTime.Now.ToString(), request.Username);
            LogHelper.Instance.Information("{@time} - Exceute import item price command for user {@user}",
                DateTime.Now.ToString(), request.Username);

            if (!FileHelpers.SaveFile(request.ImportFile, AppGlobal.UploadFileDirectory,
               out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var sizes = sizeRepository.GetSizes(request.CustomerID);
            var importResult = ForecastGroupProcess.Import(sizes.ToList(), fullPath,
                request.ImportFile.FileName, request.CustomerID, request.Username,
                out string errorMessage, out List<ForecastOverall> forcastrOveralls);

            if(importResult)
            {
                result.IsSuccess = true;
                result.Result = forcastrOveralls;
            }
            else
            {
                result.Message = errorMessage;
            }

            return result;
        }
    }
}
