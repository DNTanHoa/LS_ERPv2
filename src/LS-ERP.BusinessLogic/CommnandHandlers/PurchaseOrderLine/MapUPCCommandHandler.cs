using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
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
    public class MapUPCCommandHandler
        : IRequestHandler<MapUPCCommand, MapUPCResult>
    {
        private readonly ILogger<MapUPCCommandHandler> logger;
        private readonly IItemModelRepository itemModelRepository;
        private readonly SqlServerAppDbContext context;

        public MapUPCCommandHandler(ILogger<MapUPCCommandHandler> logger,
            IItemModelRepository itemModelRepository,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.itemModelRepository = itemModelRepository;
            this.context = context;
        }

        public async Task<MapUPCResult> Handle(MapUPCCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute map upc command", DateTime.Now.ToString());
            var result = new MapUPCResult();

            var itemModels = itemModelRepository.GetItemModels(request.CustomerID);
            var mapResult = MapUPC.Map(request.PurchaseOrderLines, itemModels.ToList(), out string errorMessage, 
                out List<PurchaseOrderLine> mappedPurchaseOrderLine);

            if (mapResult)
            {
                result.IsSuccess = true;
                result.PurchaseOrderLines = mappedPurchaseOrderLine;
            }
            else
            {
                result.Message = errorMessage;
            }

            return result;
        }
    }
}
