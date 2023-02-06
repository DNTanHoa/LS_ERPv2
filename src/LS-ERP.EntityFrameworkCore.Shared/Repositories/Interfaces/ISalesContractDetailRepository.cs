using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface ISalesContractDetailRepository
    {
        SalesContractDetail Add(SalesContractDetail salesContractDetail);
        void Update(SalesContractDetail salesContractDetail);
        void Delete(SalesContractDetail salesContractDetail);
        IQueryable<SalesContractDetail> GetSalesContractDetails();
        IQueryable<SalesContractDetail> GetSalesContractDetails(List<string> contractNos);
        SalesContractDetail GetSalesContractDetail(long ID);
        bool IsExist(long ID, out SalesContractDetail salesContractDetail);
        bool IsExist(long ID);
    }
}
