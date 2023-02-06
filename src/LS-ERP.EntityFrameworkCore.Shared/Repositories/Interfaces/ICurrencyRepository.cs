using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface ICurrencyRepository
    {
        Currency Add(Currency currency);
        void Update(Currency currency);
        void Delete(Currency currency);
        IEnumerable<Currency> GetCurrencys();
        Currency GetCurrency(string ID);
        bool IsExist(string ID, out Currency currency);
        bool IsExist(string ID);
    }
}
