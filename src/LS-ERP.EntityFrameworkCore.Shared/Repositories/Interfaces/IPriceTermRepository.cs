using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPriceTermRepository
    {
        PriceTerm Add(PriceTerm priceTerm);
        void Update(PriceTerm priceTerm);
        void Delete(PriceTerm priceTerm);
        IEnumerable<PriceTerm> GetPriceTerms();
        PriceTerm GetPriceTerm(string Code);
        bool IsExist(string Code);
        bool IsExist(string Code, out PriceTerm priceTerm);
    }
}
