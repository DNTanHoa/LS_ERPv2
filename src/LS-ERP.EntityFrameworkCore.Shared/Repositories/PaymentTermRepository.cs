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
    public class PaymentTermRepository : IPaymentTermRepository
    {
        private readonly AppDbContext context;

        public PaymentTermRepository(AppDbContext context)
        {
            this.context = context;
        }

        public PaymentTerm Add(PaymentTerm paymentTerm)
        {
            return context.PaymentTerm.Add(paymentTerm).Entity;
        }

        public void Delete(PaymentTerm paymentTerm)
        {
            context.PaymentTerm.Remove(paymentTerm);
        }

        public PaymentTerm GetPaymentTerm(string Code)
        {
            return context.PaymentTerm.FirstOrDefault(x => x.Code == Code);
        }

        public IEnumerable<PaymentTerm> GetPaymentTerms()
        {
            return context.PaymentTerm.ToList();
        }

        public bool IsExist(string Code)
        {
            var paymentTerm = GetPaymentTerm(Code);
            return paymentTerm != null ? true : false;
        }

        public bool IsExist(string Code, out PaymentTerm paymentTerm)
        {
            paymentTerm = null;
            paymentTerm = GetPaymentTerm(Code);
            return paymentTerm != null ? true : false;
        }

        public void Update(PaymentTerm paymentTerm)
        {
            context.Entry(paymentTerm).State = EntityState.Modified;
        }
    }
}
