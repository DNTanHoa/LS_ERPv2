using AutoMapper;
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
    public class ImportSalesContractCommandHandler
        : IRequestHandler<ImportSalesContractCommand, ImportSalesContractResult>
    {
        private readonly ILogger<ImportSalesContractCommandHandler> logger;
        private readonly SalesContractValidator salesContractValidator;
        private readonly ISalesContractRepository salesContractRepository;
        private readonly ISalesContractDetailRepository salesContractDetailRepository;
        private readonly IPartRepository partRepository;
        private readonly IEntitySequenceNumberRepository entitySequenceNumberRepository;
        private readonly SqlServerAppDbContext context;

        public ImportSalesContractCommandHandler(ILogger<ImportSalesContractCommandHandler> logger,
            SalesContractValidator salesContractValidator,
            ISalesContractRepository salesContractRepository,
            ISalesContractDetailRepository salesContractDetailRepository,
            IPartRepository partRepository,
            IEntitySequenceNumberRepository entitySequenceNumberRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.salesContractValidator = salesContractValidator;
            this.salesContractRepository = salesContractRepository;
            this.salesContractDetailRepository = salesContractDetailRepository;
            this.partRepository = partRepository;
            this.entitySequenceNumberRepository = entitySequenceNumberRepository;
            this.context = context;
        }
        public async Task<ImportSalesContractResult> Handle(ImportSalesContractCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute command", DateTime.Now.ToString());
            var result = new ImportSalesContractResult();
            string fileName = request.File.FileName;

            if (salesContractRepository.ExistFileSalesContract(fileName))
            {
                result.Message = "File has exist";
                return result;
            }

            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var listPart = partRepository.GetParts(request.CustomerID);

            var salesContract = ImportSalesContractProcess.Import(fullPath, fileName, request.GetUser(),
                salesContractDetailRepository.GetSalesContractDetails(), request.CustomerID, listPart,
                out List<Part> newParts, out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            if (!salesContractValidator.IsValid(salesContract, out errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            salesContract.SetCreateAudit(request.GetUser());

            var sequenceNum = entitySequenceNumberRepository
                .GetNextNumberByCode(nameof(SalesContract), out EntitySequenceNumber sequenceNumber);
            salesContract.Number += sequenceNum;
            try
            {
                context.SalesContract.Add(salesContract);
                context.Part.AddRange(newParts);
                context.Part.UpdateRange(listPart);
                context.EntitySequenceNumber.Update(sequenceNumber);

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
