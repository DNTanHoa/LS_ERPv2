using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
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
    public class UpdateSampleQuantityCommandHandler
        : IRequestHandler<UpdateSampleQuantityCommand, UpdateSampleQuantityResult>
    {
        private readonly ILogger<UpdateSampleQuantityCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IMediator mediator;

        public UpdateSampleQuantityCommandHandler(ILogger<UpdateSampleQuantityCommandHandler> logger,
            SqlServerAppDbContext context,
            IItemStyleRepository itemStyleRepository,
            IMediator mediator)
        {
            this.logger = logger;
            this.context = context;
            this.itemStyleRepository = itemStyleRepository;
            this.mediator = mediator;
        }

        public async Task<UpdateSampleQuantityResult> Handle(UpdateSampleQuantityCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Execute update simple quantity for sales order command", DateTime.Now.ToString());
            var result = new UpdateSampleQuantityResult();

            if (!FileHelpers.SaveFile(request.UpdateFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var importSimpleQuantity = UpdateSampleQuantityProcess.Import(fullPath, request.CustomerID,
                     out List<string> LSStyles,
                     out string errorMessage);

            var itemStyles = itemStyleRepository.GetItemStylesFollowLSStyle(LSStyles);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            var updateOrder = UpdateSampleQuantityProcess.UpdateSampleQuantity(importSimpleQuantity, itemStyles.ToList());

            try
            {
                context.OrderDetail.UpdateRange(updateOrder);
                //context.ItemStyleBarCode.UpdateRange(itemStyleBarcodes);
                context.SaveChanges();
                result.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.Message;
                LogHelper.Instance.Error("{@time} - Execute update simple quantity for sales order command with message {@message}",
                    DateTime.Now.ToString(), ex.InnerException?.Message);
            }

            return result;
        }
    }
}
