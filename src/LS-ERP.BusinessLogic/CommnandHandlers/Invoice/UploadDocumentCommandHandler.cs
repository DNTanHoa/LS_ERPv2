using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Dtos;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
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
    public class UploadDocumentCommandHandler
        : IRequestHandler<UploadDocumentCommand, UploadDocumentResult>
    {
        private readonly ILogger<UploadDocumentCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IMediator mediator;

        public UploadDocumentCommandHandler(ILogger<UploadDocumentCommandHandler> logger,
            SqlServerAppDbContext context,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.mediator = mediator;
        }

        public async Task<UploadDocumentResult> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute upload document invoice command", DateTime.Now.ToString());
            var result = new UploadDocumentResult();
            //string fileName = request.File.FileName;

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            DocumentFileInfoDto documentFileInfo = new DocumentFileInfoDto();
            documentFileInfo.FilePath = fullPath;
            documentFileInfo.FileName = request.File.FileName;
            documentFileInfo.FileNameServer = subPath;
            documentFileInfo.InvoiceDocumentTypeID = int.Parse(request.InvoiceDocumentTypeID);

            result.Result = documentFileInfo;
            result.IsSuccess = true;

            return result;
        }
    }
}
