using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IProductionBOMRepository
    {
        ProductionBOM Add(ProductionBOM productionBOM);
        void Update(ProductionBOM productionBOM);
        void Delete(ProductionBOM productionBOM);
        IEnumerable<ProductionBOM> GetProductionBOMs();
        IQueryable<ProductionBOM> GetProductionBOMs(List<long> IDs);
        IQueryable<ProductionBOM> GetProductionBOMsOfItemStyle(string itemStyleNumber);
        IQueryable<ProductionBOM> GetProductionBOMsOfJobHead(string jobHeadNumber);
        IQueryable<ProductionBOM> GetProductionBOMsOfItemStyles(IEnumerable<string> itemStyleNumbers);
        IQueryable<ProductionBOM> GetProductionBOMsOfContractNos(List<string> contractNos);
        IQueryable<ProductionBOM> GetProductionBOMsOfJobs(IEnumerable<string> jobHeadNumbers);
        ProductionBOM GetProductionBOM(long ID);
        bool IsExist(long ID, out ProductionBOM productionBOM);
        bool IsExist(long ID);
    }
}
