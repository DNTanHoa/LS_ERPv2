using LS_ERP.BusinessLogic.Commands;
using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Shared.Helpers;
using LS_ERP.BusinessLogic.Shared.Process;
using LS_ERP.BusinessLogic.Validator;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Global;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace LS_ERP.BusinessLogic.CommnandHandlers
{
    public class ImportPartRevisionCommandHandler
        : IRequestHandler<ImportPartRevisionCommand, ImportPartRevisionResult>
    {
        private readonly ILogger<ImportPartRevisionCommandHandler> logger;
        private readonly IPartRevisionRepository partRevisionRepository;
        private readonly IMaterialTypeRepository materialTypeRepository;
        private readonly IItemRepository itemRepository;
        private readonly PartMaterialValidator materialValidator;
        private readonly SqlServerAppDbContext context;

        public ImportPartRevisionCommandHandler(ILogger<ImportPartRevisionCommandHandler> logger,
            IPartRevisionRepository partRevisionRepository,
            IMaterialTypeRepository materialTypeRepository,
            IItemRepository itemRepository,
            PartMaterialValidator materialValidator,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.partRevisionRepository = partRevisionRepository;
            this.materialTypeRepository = materialTypeRepository;
            this.itemRepository = itemRepository;
            this.materialValidator = materialValidator;
            this.context = context;
        }

        public async Task<ImportPartRevisionResult> Handle(ImportPartRevisionCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("{@time} - Exceute import part revision command", DateTime.Now.ToString());

            var result = new ImportPartRevisionResult();

            if (!FileHelpers.SaveFile(request.ImportFile, AppGlobal.UploadFileDirectory,
                out string fullPath, out string subPath))
            {
                result.Message = "Error saving file";
                return result;
            }

            var materialType = materialTypeRepository.GetBrands().ToList();
            var items = itemRepository.GetItems();
            bool importResult = false;

            importResult = ImportPartRevisionProcess.Import(fullPath, request.Username, request.ImportFile.FileName,
                                       request.CustomerID, request.Season, materialType, items,
                                       out PartRevision partRevision,
                                       out List<PartRevision> partRevisions,
                                       out List<PartMaterial> partMaterials,
                                       out List<Item> newItems,
                                       out string errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                result.Message = errorMessage;
                return result;
            }

            if (importResult)
            {
                //partRevision.FileName = request.ImportFile.FileName;
                partRevision.FileNameServer = subPath;
                partRevision.FilePath = fullPath;
                switch (request.CustomerID)
                {
                    case "PU":
                        {
                            var bulkConfig = new BulkConfig()
                            {
                                SetOutputIdentity = true,
                                PreserveInsertOrder = true
                            };

                            var newPartMaterial = new List<PartMaterial>();
                            context.BulkInsert(newItems);

                            context.BulkInsert(partRevisions, bulkConfig);

                            foreach (var item in partRevisions)
                            {
                                foreach (PartMaterial material in partMaterials)
                                {
                                    if (item.PartNumber == material.ContractNo)
                                    {
                                        material.PartRevisionID = item.ID;
                                        material.PartNumber = item.PartNumber;
                                    }
                                }
                            }

                            await context.BulkInsertAsync(partMaterials);
                            result.IsSuccess = true;
                        }
                        break;
                    default:
                        {
                            newItems.ForEach(x =>
                            {
                                x.CustomerID = null;
                            });
                            partRevision.PartNumber = request.StyleNumber;
                            partRevision.RevisionNumber = request.RevisionNumber;
                            partRevision.IsConfirmed = request.IsConfirmed;

                            partRevision.SetCreateAudit(request.Username);

                            if (!materialValidator.IsValid(partRevision.PartMaterials.ToList(),
                                out errorMessage))
                            {
                                result.Message = errorMessage;
                                return result;
                            }

                            result.Items = newItems;
                            result.PartRevision = partRevision;
                            result.IsSuccess = true;

                            //context.Item.AddRange(newItems);
                            context.SaveChanges();
                        }
                        break;
                }

                /// Update Group Size for Part Material
                if (request.CustomerID == "DE")
                {
                    partRevision.PartMaterials.ToList().ForEach(x =>
                    {
                        if (!((x.ItemName ?? string.Empty).ToUpper().Contains("RFID")) &&
                             string.IsNullOrEmpty(x.Specify) &&
                             x.GroupSize != true)
                        {
                            x.GroupSize = true;
                        }

                        if (((x.ItemName ?? string.Empty).ToUpper().Contains("ADEQUAT") ||
                             (x.ItemName ?? string.Empty).ToUpper().Contains("LFT ADQ") ||
                             (x.ItemName ?? string.Empty).ToUpper().Contains("STK ADQ") ||
                             (x.ItemName ?? string.Empty).ToUpper().Contains("TAG ADQ")) &&
                             x.GroupItemColor != true)
                        {
                            x.GroupItemColor = true;
                        }
                    });
                }
                else if (request.CustomerID == "GA")
                {
                    partRevision.PartMaterials.ToList().ForEach(x =>
                    {
                        if (!(x.ItemName ?? string.Empty).ToUpper().Replace(" ", "").Contains("CARELABEL") &&
                            !(x.ItemName ?? string.Empty).ToUpper().Replace(" ", "").Contains("SIZELABEL") &&
                            x.GroupSize != true)
                        {
                            x.GroupSize = true;
                        }
                    });
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
