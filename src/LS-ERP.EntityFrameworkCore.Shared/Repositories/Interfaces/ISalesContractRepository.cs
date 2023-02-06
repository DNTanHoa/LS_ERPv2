using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface ISalesContractRepository
    {
        SalesContract Add(SalesContract salesContract);
        void Update(SalesContract salesContract);
        void Delete(SalesContract salesContract);
        IQueryable<SalesContract> GetSalesContracts();
        IQueryable<SalesContract> GetSalesContracts(string customerID);
        SalesContract GetSalesContract(string ID);
        SalesContract GetNewSalesContract(string customerID);
        bool IsExist(string ID);
        bool ExistFileSalesContract(string fileName);

    }
}
