using Hangfire;
using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.SqlServer.Context;
using LS_ERP.Ultilities.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Shared.Process.Jobs
{
    public class UpdateGroupSizeProductionBOMJob
    {
        private readonly ILogger<UpdateGroupSizeProductionBOMJob> logger;
        private readonly SqlServerAppDbContext context;

        public UpdateGroupSizeProductionBOMJob(ILogger<UpdateGroupSizeProductionBOMJob> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Update Group Size Production Bom Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute(List<ProductionBOM> addProductionBOMs, List<ProductionBOM> updateProductionBOMs)
        {
            try
            {
                addProductionBOMs.ForEach(x =>
                {
                    var partMaterial = context.PartMaterial
                           .FirstOrDefault(p => p.ID.ToString() == x.PartMaterialID);
                    if (partMaterial != null)
                    {
                        x.GroupSize = partMaterial.GroupSize;
                    }
                });

                updateProductionBOMs.ForEach(x =>
                {
                    var partMaterial = context.PartMaterial
                           .FirstOrDefault(p => p.ID.ToString() == x.PartMaterialID);
                    if (partMaterial != null)
                    {
                        x.GroupSize = partMaterial.GroupSize;
                    }
                });

                context.ProductionBOM.UpdateRange(addProductionBOMs);
                context.ProductionBOM.UpdateRange(updateProductionBOMs);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogError("{@time} - Update group size master bom handler has error {@error}",
                   DateTime.Now, ex.InnerException.Message);
                LogHelper.Instance.Error("{@time} - Update group size master bom handler has error {@error}",
                    DateTime.Now, ex.InnerException.Message);
            }

            return Task.CompletedTask;
        }
    }
}
