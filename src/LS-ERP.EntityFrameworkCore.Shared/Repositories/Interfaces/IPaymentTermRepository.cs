using LS_ERP.EntityFrameworkCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces
{
    public interface IPaymentTermRepository
    {
        PaymentTerm Add(PaymentTerm paymentTerm);
        void Update(PaymentTerm paymentTerm);
        void Delete(PaymentTerm paymentTerm);
        IEnumerable<PaymentTerm> GetPaymentTerms();
        PaymentTerm GetPaymentTerm(string Code);
        bool IsExist(string Code);
        bool IsExist(string Code, out PaymentTerm paymentTerm);
    }
}
