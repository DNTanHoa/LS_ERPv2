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
    public class PriceTermRepository : IPriceTermRepository
    {
        private readonly AppDbContext context;

        public PriceTermRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PriceTerm Add(PriceTerm priceTerm)
        {
            return context.PriceTerm.Add(priceTerm).Entity;
        }

        public void Delete(PriceTerm priceTerm)
        {
            context.PriceTerm.Remove(priceTerm);
        }

        public PriceTerm GetPriceTerm(string Code)
        {
            return context.PriceTerm.FirstOrDefault(x => x.Code == Code);
        }

        public IEnumerable<PriceTerm> GetPriceTerms()
        {
            return context.PriceTerm.ToList();
        }

        public bool IsExist(string Code)
        {
            var priceTerm = GetPriceTerm(Code);
            return priceTerm != null ? true : false;
        }

        public bool IsExist(string Code, out PriceTerm priceTerm)
        {
            priceTerm = null;
            priceTerm = GetPriceTerm(Code);
            return priceTerm != null ? true : false;
        }

        public void Update(PriceTerm priceTerm)
        {
            context.Entry(priceTerm).State = EntityState.Modified;
        }
    }
}
