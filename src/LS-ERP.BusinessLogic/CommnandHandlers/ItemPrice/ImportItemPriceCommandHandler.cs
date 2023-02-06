using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Common;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
using LS_ERP.Ultilities.Helpers;
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
    public class ImportItemPriceCommandHandler
        : IRequestHandler<ImportItemPriceCommand, CommonCommandResult<List<ItemPrice>>>
    {
        private readonly ILogger<ImportItemPriceCommandHandler> logger;
        private readonly IVendorRepository vendorRepository;
        private readonly IUnitRepository unitRepository;
        private readonly IItemPriceRepository itemPriceRepository;
        private readonly IItemRepository itemRepository;
        private readonly ItemPriceValidator validator;
        private readonly SqlServerAppDbContext context;

        public ImportItemPriceCommandHandler(ILogger<ImportItemPriceCommandHandler> logger,
            IVendorRepository vendorRepository,
            IUnitRepository unitRepository,
            IItemPriceRepository itemPriceRepository,
            IItemRepository itemRepository,
            ItemPriceValidator validator,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.vendorRepository = vendorRepository;
            this.unitRepository = unitRepository;
            this.itemPriceRepository = itemPriceRepository;
            this.itemRepository = itemRepository;
            this.validator = validator;
            this.context = context;
        }

        public async Task<CommonCommandResult<List<ItemPrice>>> Handle(ImportItemPriceCommand request,
            CancellationToken cancellationToken)
        {
            var result = new CommonCommandResult<List<ItemPrice>>();
            logger.LogInformation("{@time} - Exceute import item price command", DateTime.Now.ToString());
            LogHelper.Instance.Information("{@time} - Exceute import item price command for user {@user}",
                DateTime.Now.ToString(), request.Username);

            if (!FileHelpers.SaveFile(request.ImportFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var shippingTerms = context.ShippingTerm.ToList();
            var items = itemRepository.GetItems(request.CustomerID);
            var importResult = ImportItemPriceProcess.Import(fullPath, request.ImportFile.FileName, request.Username,
                request.CustomerID,
                shippingTerms, items.ToList(),
                out string errorMessage, out List<ItemPrice> itemPrices, out List<Item> newItems);

            if (importResult)
            {
                if (validator.IsValid(itemPrices, out errorMessage))
                {
                    result.Result = itemPrices;
                    result.IsSuccess = true;

                    await context.Item.AddRangeAsync(newItems);
                    await context.SaveChangesAsync(cancellationToken);

                    return result;
                }
                else
                {
                    result.Message = errorMessage;
                }
            }
            else
            {
                result.Message = errorMessage;
            }

            return result;
        }
    }
}
