using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
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
    public class MergeItemCommandHandler
        : IRequestHandler<MergeItemCommand, MergeItemResult>
    {
        private readonly ILogger<MergeItemCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IItemRepository itemRepository;
        private readonly IPartRevisionRepository partRevisionRepository;

        public MergeItemCommandHandler(ILogger<MergeItemCommandHandler> logger,
            SqlServerAppDbContext context,
            IItemRepository itemRepository,
            IPartRevisionRepository partRevisionRepository)
        {
            this.logger = logger;
            this.context = context;
            this.itemRepository = itemRepository;
            this.partRevisionRepository = partRevisionRepository;
        }
        public async Task<MergeItemResult> Handle(MergeItemCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute merge item command", DateTime.Now.ToString());
            var result = new MergeItemResult();

            var partRevisions = partRevisionRepository.GetPartRevisions();
            var items = itemRepository.GetItems();
            //ImportPartRevisionProcess.MergeItems(request.Username, partRevisions);

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
