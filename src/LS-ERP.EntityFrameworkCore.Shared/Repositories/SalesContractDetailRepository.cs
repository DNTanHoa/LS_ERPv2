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
    public class SalesContractDetailRepository : ISalesContractDetailRepository
    {
        private readonly AppDbContext context;

        public SalesContractDetailRepository(AppDbContext context)
        {
            this.context = context;
        }

        public SalesContractDetail Add(SalesContractDetail SalesContractDetail)
        {
            return context.SalesContractDetail.Add(SalesContractDetail).Entity;
        }

        public void Delete(SalesContractDetail SalesContractDetail)
        {
            context.SalesContractDetail.Remove(SalesContractDetail);
        }

        public IQueryable<SalesContractDetail> GetSalesContractDetails()
        {
            return context.SalesContractDetail.Where(x => x.CreatedAt >= DateTime.Now.AddYears(-2));
        }

        public SalesContractDetail GetSalesContractDetail(long ID)
        {
            return context.SalesContractDetail.FirstOrDefault(x => x.ID == ID);
        }

        public bool IsExist(long ID, out SalesContractDetail SalesContractDetail)
        {
            SalesContractDetail = null;
            SalesContractDetail = GetSalesContractDetail(ID);
            return SalesContractDetail != null ? true : false;
        }

        public bool IsExist(long ID)
        {
            var SalesContractDetail = GetSalesContractDetail(ID);
            return SalesContractDetail != null ? true : false;
        }

        public void Update(SalesContractDetail SalesContractDetail)
        {
            context.Entry(SalesContractDetail).State = EntityState.Modified;
        }

        public IQueryable<SalesContractDetail> GetSalesContractDetails(List<string> contractNos)
        {
            return context.SalesContractDetail.Where(x => contractNos.Contains(x.ContractNo));
        }
    }
}
