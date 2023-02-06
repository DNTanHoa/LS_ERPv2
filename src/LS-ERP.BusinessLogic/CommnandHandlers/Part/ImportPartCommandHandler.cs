using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
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
    public class ImportPartCommandHandler
         : IRequestHandler<ImportPartCommand, CommonCommandResult<Part>>
    {
        private readonly ILogger<ImportPartCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IPartRepository partRepository;
        private readonly IMapper mapper;

        public ImportPartCommandHandler(ILogger<ImportPartCommandHandler> logger,
            SqlServerAppDbContext context,
            IItemStyleRepository itemStyleRepository,
            IPartRepository partRepository,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.itemStyleRepository = itemStyleRepository;
            this.partRepository = partRepository;
            this.mapper = mapper;
        }

        public async Task<CommonCommandResult<Part>> Handle(ImportPartCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import part command", DateTime.Now.ToString());

            var result = new CommonCommandResult<Part>();

            if (!FileHelpers.SaveFile(request.FilePath, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var saleOrderIDs = request.SalesOrderIDs.Split(';');
            var oldPart = partRepository.GetParts(request.CustomerID).ToList();
            var oldItemStyles = itemStyleRepository.GetOnlyItemStylesFollowSalesOrderID(saleOrderIDs.ToList()).ToList();

            ImportPartProcess.Import(fullPath, request.UserName, oldItemStyles,
                request.Update,
                ref oldPart,
                out List<Part> newParts,
                out List<Part> deleteParts,
                out List<ItemStyle> updateItemStyles,
                out List<ItemStyle> newGenItemStyles,
                out string errorMessage);


            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                switch (request.CustomerID)
                {
                    case "HA":
                        if (!request.Update)
                        {
                            context.Part.RemoveRange(deleteParts);
                            context.Part.AddRange(newParts);
                            await context.SaveChangesAsync(cancellationToken);
                            result.IsSuccess = true;

                            result.Result = newParts.FirstOrDefault();
                        }
                        else
                        {
                            context.Part.UpdateRange(oldPart);
                            context.Part.AddRange(newParts);
                            context.ItemStyle.UpdateRange(updateItemStyles);
                            context.ItemStyle.UpdateRange(newGenItemStyles);

                            await context.SaveChangesAsync(cancellationToken);
                            result.IsSuccess = true;

                            result.Result = new Part();
                        }

                        break;
                    default:

                        break;
                }


            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
