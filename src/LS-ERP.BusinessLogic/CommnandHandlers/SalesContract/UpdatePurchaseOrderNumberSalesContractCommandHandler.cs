using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class UpdatePurchaseOrderNumberSalesContractCommandHandler
        : IRequestHandler<UpdatePurchaseOrderNumberSalesContractCommand, ImportSalesContractResult>
    {
        private readonly ILogger<UpdatePurchaseOrderNumberSalesContractCommand> logger;
        private readonly SqlServerAppDbContext context;
        private readonly ISalesContractRepository salesContractRepository;
        private readonly ISalesContractDetailRepository salesContractDetailRepository;
        private readonly IPartRepository partRepository;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly SalesContractValidator salesContractValidator;

        public UpdatePurchaseOrderNumberSalesContractCommandHandler(ILogger<UpdatePurchaseOrderNumberSalesContractCommand> logger,
            SqlServerAppDbContext context,
            ISalesContractRepository salesContractRepository,
            ISalesContractDetailRepository salesContractDetailRepository,
            IPartRepository partRepository,
            IItemStyleRepository itemStyleRepository,
            SalesContractValidator salesContractValidator)
        {
            this.logger = logger;
            this.context = context;
            this.salesContractRepository = salesContractRepository;
            this.salesContractDetailRepository = salesContractDetailRepository;
            this.partRepository = partRepository;
            this.itemStyleRepository = itemStyleRepository;
            this.salesContractValidator = salesContractValidator;
        }
        public async Task<ImportSalesContractResult> Handle(UpdatePurchaseOrderNumberSalesContractCommand request, CancellationToken cancellationToken)
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

            var currentSalesContractDetails = salesContractDetailRepository.GetSalesContractDetails();
            List<SalesContractDetail> newSalesContractDetail = new List<SalesContractDetail>();
            var itemStyles = itemStyleRepository.GetItemStyles(request.CustomerID);
            UpdateSalesContractProcess.UpdatePurchaseOrderNumberSalesContract(currentSalesContractDetails, itemStyles,
                fullPath, fileName,
                request.GetUser(), request.CustomerID,
                out newSalesContractDetail,
                out List<ItemStyle> updateItemStyle,
                out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }


            try
            {
                context.SalesContractDetail.UpdateRange(newSalesContractDetail);
                if (updateItemStyle != null && updateItemStyle.Count > 0)
                {
                    context.ItemStyle.UpdateRange(updateItemStyle);
                }

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
