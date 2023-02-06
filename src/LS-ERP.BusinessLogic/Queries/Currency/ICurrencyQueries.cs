using LS_ERP.BusinessLogic.Dtos.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface ICurrencyQueries
    {
        IEnumerable<CurrencyDtos> GetCurrencies();
        CurrencyDtos GetCurrency(string ID);
    }
}
