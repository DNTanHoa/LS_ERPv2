using Common.Model;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class UpdateSaleOrdersInformationCommandHandler
        : IRequestHandler<UpdateSaleOrdersInformationCommand,
            CommonCommandResultHasData<IEnumerable<ItemStyleInforDtos>>>
    {
        private readonly ILogger<UpdateSaleOrdersInformationCommandHandler> logger;

        public UpdateSaleOrdersInformationCommandHandler(
            ILogger<UpdateSaleOrdersInformationCommandHandler> logger)
        {
            this.logger = logger;
        }
        public async Task<CommonCommandResultHasData<IEnumerable<ItemStyleInforDtos>>> Handle(
            UpdateSaleOrdersInformationCommand request, CancellationToken cancellationToken)
        {
            var result = new CommonCommandResultHasData<IEnumerable<ItemStyleInforDtos>>();

            logger.LogInformation("{@time} - Exceute import item price command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute import item price command for user {@user}",
                DateTime.Now.ToString(), request.UserName);

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            bool importResult = false;
            string errorMessage = string.Empty;
            List<dynamic> data = null;

            (importResult, errorMessage, data) = UpdateInforProcess.Import(fullPath);

            if(importResult)
            {
                var json = JsonSerializer.Serialize(data);
                var resultData = JsonSerializer.Deserialize<List<ItemStyleInforDtos>>(json);

                result.Success = true;
                result.Data = resultData;
            }
            else
            {
                result.Message = errorMessage;
            }

            return result;
        }
    }
}
