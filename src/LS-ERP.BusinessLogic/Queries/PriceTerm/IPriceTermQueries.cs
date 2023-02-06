using LS_ERP.BusinessLogic.Dtos.PriceTerm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IPriceTermQueries
    {
        IEnumerable<PriceTermDtos> GetPriceTerms();
        PriceTermDtos GetPriceTerm(string Code);
    }
}
