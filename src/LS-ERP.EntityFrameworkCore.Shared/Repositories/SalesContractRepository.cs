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
    public class SalesContractRepository
        : ISalesContractRepository
    {
        private readonly AppDbContext context;

        public SalesContractRepository(AppDbContext context)
        {
            this.context = context;
        }
        public SalesContract Add(SalesContract salesContract)
        {
            return context.SalesContract.Add(salesContract).Entity;
        }

        public void Delete(SalesContract salesContract)
        {
            context.SalesContract.Remove(salesContract);
        }

        public bool ExistFileSalesContract(string fileName)
        {
            var salesContract = context.SalesContract.FirstOrDefault(x => x.FileName == fileName);
            return salesContract != null ? true : false;
        }

        public SalesContract GetNewSalesContract(string customerID)
        {
            return context.SalesContract
               .Include(x => x.ContractDetails)
               .OrderByDescending(x => x.CreatedAt)
               .FirstOrDefault(x => x.CustomerID == customerID);
        }

        public SalesContract GetSalesContract(string ID)
        {
            return context.SalesContract
                .Include(x => x.ContractDetails)
                .AsSingleQuery()
                .FirstOrDefault(x => x.ID == ID);
        }

        public IQueryable<SalesContract> GetSalesContracts()
        {
            return context.SalesContract;
        }

        public IQueryable<SalesContract> GetSalesContracts(string customerID)
        {
            return context.SalesContract
                .Include(x => x.ContractDetails)
                .Where(x => x.CustomerID == customerID);
        }

        public bool IsExist(string ID)
        {
            var contract = context.SalesContract.FirstOrDefault(x => x.ID == ID);
            return contract != null ? true : false;
        }

        public void Update(SalesContract salesContract)
        {
            context.Entry(salesContract).State = EntityState.Modified;
        }
    }
}
