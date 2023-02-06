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
    public class UploadCommandHandler : IRequestHandler<UploadCommand, UploadResult>
    {
        private readonly ILogger<UploadCommandHandler> logger;

        public UploadCommandHandler(ILogger<UploadCommandHandler> logger)
        {
            this.logger = logger;
        }

        public async Task<UploadResult> Handle(UploadCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Execute upload file command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Execute upload file command", DateTime.Now.ToString());

            var result = new UploadResult();

            if (FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.IsSuccess = true;
                result.Url = subPath;
            }
            else
            {
                result.Message = "Error saving file";
                return result;
            }

            return result;
        }
    }
}
