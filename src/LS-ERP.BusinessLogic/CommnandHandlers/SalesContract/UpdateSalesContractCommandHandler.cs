
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
    public class UpdateSalesContractCommandHandler
        : IRequestHandler<UpdateSalesContractCommand, ImportSalesContractResult>
    {
        private readonly ILogger<UpdateSalesContractCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly ISalesContractRepository salesContractRepository;
        private readonly ISalesContractDetailRepository salesContractDetailRepository;
        private readonly IPartRepository partRepository;
        private readonly SalesContractValidator salesContractValidator;

        public UpdateSalesContractCommandHandler(ILogger<UpdateSalesContractCommandHandler> logger,
            SqlServerAppDbContext context,
            ISalesContractRepository salesContractRepository,
            ISalesContractDetailRepository salesContractDetailRepository,
            IPartRepository partRepository,
            SalesContractValidator salesContractValidator)
        {
            this.logger = logger;
            this.context = context;
            this.salesContractRepository = salesContractRepository;
            this.salesContractDetailRepository = salesContractDetailRepository;
            this.partRepository = partRepository;
            this.salesContractValidator = salesContractValidator;
        }
        public async Task<ImportSalesContractResult> Handle(UpdateSalesContractCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute update sales contract command", DateTime.Now.ToString());
            var result = new ImportSalesContractResult();
            string fileName = request.File.FileName;
            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var listPart = partRepository.GetParts(request.CustomerID);
            var currentSalesContract = salesContractRepository.GetSalesContract(request.SalesContractID);
            List<SalesContractDetail> newSalesContractDetail = new List<SalesContractDetail>();

            UpdateSalesContractProcess.Update(currentSalesContract, fullPath, fileName, request.GetUser(),
                salesContractDetailRepository.GetSalesContractDetails(), request.CustomerID, listPart,
                out List<Part> newParts, ref newSalesContractDetail, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            if (!salesContractValidator.IsValid(currentSalesContract, out errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                context.Part.AddRange(newParts);
                context.Part.UpdateRange(listPart);
                context.SalesContractDetail.AddRange(newSalesContractDetail);
                salesContractRepository.Update(currentSalesContract);

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
