using AutoMapper;
using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
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
    public class ImportItemModelCommandHandler
        : IRequestHandler<ImportItemModelCommand, ImportItemModelResult>
    {
        private readonly ILogger<ImportItemModelCommandHandler> logger;
        private readonly SqlServerAppDbContext context;
        private readonly IItemModelRepository itemModelRepository;
        private readonly ISizeRepository sizeRepository;
        private readonly IItemStyleRepository itemStyleRepository;

        public ImportItemModelCommandHandler(ILogger<ImportItemModelCommandHandler> logger,
            SqlServerAppDbContext context,
            IItemModelRepository itemModelRepository,
            ISizeRepository sizeRepository,
            IItemStyleRepository itemStyleRepository)
        {
            this.logger = logger;
            this.context = context;
            this.itemModelRepository = itemModelRepository;
            this.sizeRepository = sizeRepository;
            this.itemStyleRepository = itemStyleRepository;
        }

        public async Task<ImportItemModelResult> Handle(ImportItemModelCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute Import Item Model command", DateTime.Now.ToString());
            var result = new ImportItemModelResult();
            string fileName = request.File.FileName;

            if (itemModelRepository.IsExistFile(fileName))
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

            var sizes = sizeRepository.GetSizes(request.CustomerID);

            ItemModelProcess.ImportItemModel(fullPath, fileName, request.Username,
                request.CustomerID, sizes,
                out List<ItemModel> newItemModel,
                out Dictionary<string, Dictionary<string, ItemModel>> dicLSStyles,
                out string errorMessage);

            var updateItemBarcodes = new List<ItemStyleBarCode>();

            switch (request.CustomerID)
            {
                case "IFG":
                    {
                        var config = new MapperConfiguration(
                          cfg => cfg.CreateMap<ItemStyleBarCode, ItemStyleBarCode>().ForMember(d => d.ItemStyle, o => o.Ignore()));
                        var mapper = new Mapper(config);

                        var itemStyles = itemStyleRepository.GetItemStylesFollowLSStyleBarcodes(dicLSStyles.Keys.ToList(), "IFG");
                        foreach (var itemStyle in itemStyles)
                        {
                            if (dicLSStyles.TryGetValue(itemStyle.LSStyle, out Dictionary<string, ItemModel> rsDicBarcode))
                            {
                                foreach (var barcode in itemStyle.Barcodes)
                                {
                                    if (rsDicBarcode.TryGetValue(itemStyle.LSStyle + barcode.Size, out ItemModel rsItemModel))
                                    {
                                        ItemStyleBarCode itemStyleBarCode = new ItemStyleBarCode();
                                        mapper.Map(barcode, itemStyleBarCode);

                                        itemStyleBarCode.Color = rsItemModel.CustomerColorCode;
                                        itemStyleBarCode.BarCode = rsItemModel.Barcode;

                                        updateItemBarcodes.Add(itemStyleBarCode);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            try
            {
                //context.SalesOrders.Add(salesOrder);
                //context.Part.AddRange(newParts);
                //context.Part.UpdateRange(parts);
                //context.EntitySequenceNumber.Update(sequenceNumber);
                //context.PurchaseOrderType.AddRange(newPurchaseOrderTypes);

                context.ItemModel.AddRange(newItemModel);

                if (updateItemBarcodes.Any())
                {
                    context.ItemStyleBarCode.UpdateRange(updateItemBarcodes);
                }

                await context.SaveChangesAsync(cancellationToken);
                result.IsSuccess = true;

                result.Result = newItemModel;
            }
            catch (DbUpdateException ex)
            {
                result.Message = ex.InnerException.Message;
            }

            return result;
        }
    }
}
