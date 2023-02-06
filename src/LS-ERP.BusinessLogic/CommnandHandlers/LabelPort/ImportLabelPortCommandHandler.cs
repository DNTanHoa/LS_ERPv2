using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
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
    public class ImportLabelPortCommandHandler
        : IRequestHandler<ImportLabelPortCommand, ImportLabelPortResult>
    {
        private readonly ILogger<ImportLabelPortCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IItemStyleRepository itemStyleRepository;
        private readonly IMapper mapper;

        public ImportLabelPortCommandHandler(ILogger<ImportLabelPortCommandHandler> logger,
            SqlServerAppDbContext context,
            IItemStyleRepository itemStyleRepository,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.itemStyleRepository = itemStyleRepository;
            this.mapper = mapper;
        }

        public async Task<ImportLabelPortResult> Handle(ImportLabelPortCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import label port command", DateTime.Now.ToString());

            var result = new ImportLabelPortResult();

            if (!FileHelpers.SaveFile(request.FilePath, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var oldItemStyles = itemStyleRepository.GetItemStyles(request.CustomerID).ToList();

            ImportLabelPortProcess.Import(fullPath, request.Username, request.CustomerID,
                out List<LabelPort> newLabelPort,
                out Dictionary<string, LabelPort> dicLabelPort,
                out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            foreach (var itemStyle in oldItemStyles)
            {
                string key = itemStyle.Division + itemStyle.LabelCode;
                if (dicLabelPort.TryGetValue(key, out LabelPort rsLabelPort))
                {
                    itemStyle.ETAPort = rsLabelPort.ETAPort;
                }
                else if (!int.TryParse(itemStyle.Division, out int division))
                {
                    itemStyle.ETAPort = "NEW JERSEY";
                }
            }

            try
            {
                switch (request.CustomerID)
                {
                    case "HA":
                        if (newLabelPort.Any())
                        {
                            context.LabelPort.AddRange(newLabelPort);
                            context.ItemStyle.UpdateRange(oldItemStyles);
                            await context.SaveChangesAsync(cancellationToken);
                            result.IsSuccess = true;

                            result.Result = newLabelPort;
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
