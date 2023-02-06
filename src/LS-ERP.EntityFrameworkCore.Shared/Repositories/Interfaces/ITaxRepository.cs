using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface ITaxRepository
    {
        Tax Add(Tax tax);
        void Update(Tax tax);
        void Delete(Tax tax);
        IEnumerable<Tax> GetTaxs();
        Tax GetTax(string code);
        bool IsExist(string code);
    }
}
