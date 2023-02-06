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
    public class UpdateGroupSizeMasterBomJob
    {
        private readonly ILogger<UpdateGroupSizeMasterBomJob> logger;
        private readonly SqlServerAppDbContext context;

        public UpdateGroupSizeMasterBomJob(ILogger<UpdateGroupSizeMasterBomJob> logger,
            SqlServerAppDbContext context)
        {
            this.logger = logger;
            this.context = context;
        }

        [JobDisplayName("Update Group Size Master Bom Job")]
        [AutomaticRetry(Attempts = 3)]
        public Task Execute()
        {
            try
            {
                var updateProductionBOMs = new List<ProductionBOM>();
                var partMaterials = context.PartMaterial
                        .Where(x => !x.ItemName.Contains("RFID") &&
                               x.GroupSize != true).ToList();
                partMaterials.ForEach(x =>
                {
                    var productionBOMs = context.ProductionBOM
                        .Where(y => y.PartMaterialID == x.ID.ToString()).ToList();
                    foreach (var data in productionBOMs)
                    {
                        data.GroupSize = true;
                        updateProductionBOMs.Add(data);
                    }
                    x.GroupSize = true;
                });

                context.PartMaterial.UpdateRange(partMaterials);
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
