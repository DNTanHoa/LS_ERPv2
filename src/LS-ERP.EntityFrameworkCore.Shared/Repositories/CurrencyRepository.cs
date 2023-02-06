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
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly AppDbContext context;

        public CurrencyRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Currency Add(Currency currency)
        {
            return context.Currency.Add(currency).Entity;
        }

        public void Delete(Currency currency)
        {
            context.Currency.Remove(currency);
        }

        public Currency GetCurrency(string ID)
        {
            return context.Currency.FirstOrDefault(x => x.ID == ID);
        }

        public IEnumerable<Currency> GetCurrencys()
        {
            return context.Currency.ToList();
        }

        public bool IsExist(string ID)
        {
            var currency = GetCurrency(ID);
            return currency != null ? true : false;
        }

        public bool IsExist(string ID, out Currency currency)
        {
            currency = null;
            currency = GetCurrency(ID);
            return currency != null ? true : false; throw new NotImplementedException();
        }

        public void Update(Currency customer)
        {
            context.Entry(customer).State = EntityState.Modified;
        }
    }
}
