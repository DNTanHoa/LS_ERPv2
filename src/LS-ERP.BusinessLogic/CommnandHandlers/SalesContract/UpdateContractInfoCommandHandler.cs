using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
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
    public class UpdateContractInfoCommandHandler
        : IRequestHandler<UpdateContractInfoCommand, ImportSalesContractResult>
    {
        private readonly ILogger<UpdateContractInfoCommand> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly ISizeRepository sizeRepository;
        private readonly ISalesContractDetailRepository salesContractDetailRepository;

        public UpdateContractInfoCommandHandler(ILogger<UpdateContractInfoCommand> logger,
            SqlServerAppDbContext context,
            IItemStyleRepository itemStyleRepository,
            ISizeRepository sizeRepository,
            ISalesContractDetailRepository salesContractDetailRepository)
        {
            this.logger = logger;
            this.context = context;
            this.itemStyleRepository = itemStyleRepository;
            this.sizeRepository = sizeRepository;
            this.salesContractDetailRepository = salesContractDetailRepository;
        }

        public async Task<ImportSalesContractResult> Handle(UpdateContractInfoCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute update contract info command", DateTime.Now.ToString());
            var result = new ImportSalesContractResult();
            string fileName = request.File.FileName;
            if (!FileHelpers.SaveFile(request.File, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }


            var dicImportItemStyles = UpdateContractInfoProcessor.Import(fullPath, fileName, request.CustomerID,
                out List<string> contractNos,
                out Dictionary<string, List<Dictionary<string, OrderDetail>>> importDicOrderDetails,
                out string errorMessage);

            var oldItemStyle = itemStyleRepository.GetItemStylesForContractInfo(contractNos)?.ToList();
            var sizes = sizeRepository.GetSizes(request.CustomerID);
            var salesContractDetails = salesContractDetailRepository.GetSalesContractDetails(contractNos)?.ToList();

            UpdateContractInfoProcessor.CompareData(request.UserName, oldItemStyle, dicImportItemStyles, importDicOrderDetails, sizes,
                ref salesContractDetails,
                out List<ItemStyle> updateItemStyles,
                out List<OrderDetail> newOrderDetails,
                out List<OrderDetail> updateOrderDetails,
                out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }


            try
            {
                context.SalesContractDetail.UpdateRange(salesContractDetails);

                if (newOrderDetails.Any())
                {
                    context.OrderDetail.AddRange(newOrderDetails);
                }
                context.OrderDetail.UpdateRange(updateOrderDetails);
                context.ItemStyle.UpdateRange(updateItemStyles);

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
