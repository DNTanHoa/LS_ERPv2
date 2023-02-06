using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class MergeItemModelCommandHandler
        : IRequestHandler<MergeItemModelCommand, MergeItemModelResult>
    {
        private readonly ILogger<ImportItemModelCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IItemModelRepository itemModelRepository;
        private readonly ISizeRepository sizeRepository;

        public MergeItemModelCommandHandler(ILogger<ImportItemModelCommandHandler> logger,
            SqlServerAppDbContext context,
            IItemModelRepository itemModelRepository,
            ISizeRepository sizeRepository)
        {
            this.logger = logger;
            this.context = context;
            this.itemModelRepository = itemModelRepository;
            this.sizeRepository = sizeRepository;
        }

        public async Task<MergeItemModelResult> Handle(MergeItemModelCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute Import Item Model command", DateTime.Now.ToString());
            var result = new MergeItemModelResult();


            //var sizes = sizeRepository.GetSizes(request.CustomerID);
            //ItemModelProcess.ImportItemModel(request.Username,
            //    request.CustomerID, sizes,
            //    out List<ItemModel> newItemModel,
            //    out string errorMessage);

            //if (!string.IsNullOrEmpty(errorMessage))
            //{
            //    result.Message = errorMessage;
            //    return result;
            //}

            try
            {
                //context.SalesOrders.Add(salesOrder);
                //context.Part.AddRange(newParts);
                //context.Part.UpdateRange(parts);
                //context.EntitySequenceNumber.Update(sequenceNumber);
                //context.PurchaseOrderType.AddRange(newPurchaseOrderTypes);

                //context.ItemModel.AddRange(newItemModel);

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
