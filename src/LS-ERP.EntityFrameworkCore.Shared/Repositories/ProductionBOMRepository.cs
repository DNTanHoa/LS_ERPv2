using LS_ERP.EntityFrameworkCore.Entities;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories
{
    public class ProductionBOMRepository : IProductionBOMRepository
    {
        private readonly AppDbContext context;

        public ProductionBOMRepository(AppDbContext context)
        {
            this.context = context;
        }

        public ProductionBOM Add(ProductionBOM productionBOM)
        {
            return context.ProductionBOM.Add(productionBOM).Entity;
        }

        public void Delete(ProductionBOM productionBOM)
        {
            context.ProductionBOM.Remove(productionBOM);
        }

        public ProductionBOM GetProductionBOM(long ID)
        {
            return context.ProductionBOM.FirstOrDefault(x => x.ID == ID);
        }

        public IEnumerable<ProductionBOM> GetProductionBOMs()
        {
            return context.ProductionBOM;
        }

        public IQueryable<ProductionBOM> GetProductionBOMs(List<long> IDs)
        {
            return context.ProductionBOM.Where(x => IDs.Contains(x.ID));
        }

        public IQueryable<ProductionBOM> GetProductionBOMsOfContractNos(List<string> contractNos)
        {
            return context.ProductionBOM
                          //.Include(x => x.ItemStyle)
                          .Where(x => contractNos.Contains(x.ContractNo));
        }

        public IQueryable<ProductionBOM> GetProductionBOMsOfItemStyle(string itemStyleNumber)
        {
            return context.ProductionBOM.Where(x => x.ItemStyleNumber == itemStyleNumber);
        }

        public IQueryable<ProductionBOM> GetProductionBOMsOfItemStyles(IEnumerable<string> itemStyleNumbers)
        {
            return context.ProductionBOM.Where(x => itemStyleNumbers.Contains(x.ItemStyleNumber));
        }

        public IQueryable<ProductionBOM> GetProductionBOMsOfJobHead(string jobHeadNumber)
        {
            return context.ProductionBOM.Where(x => x.JobHeadNumber == jobHeadNumber);
        }

        public IQueryable<ProductionBOM> GetProductionBOMsOfJobs(IEnumerable<string> jobHeadNumbers)
        {
            return context.ProductionBOM.Where(x => jobHeadNumbers.Contains(x.JobHeadNumber));
        }

        public bool IsExist(long ID, out ProductionBOM productionBOM)
        {
            productionBOM = null;
            productionBOM = GetProductionBOM(ID);
            return productionBOM != null ? true : false;
        }

        public bool IsExist(long ID)
        {
            var productionBOM = GetProductionBOM(ID);
            return productionBOM != null ? true : false;
        }

        public void Update(ProductionBOM productionBOM)
        {
            context.Entry(productionBOM).State = EntityState.Modified;
        }
    }
}
