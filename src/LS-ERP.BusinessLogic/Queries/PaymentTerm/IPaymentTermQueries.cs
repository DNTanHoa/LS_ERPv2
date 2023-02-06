using LS_ERP.BusinessLogic.Dtos.PaymentTerm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IPaymentTermQueries
    {
        IEnumerable<PaymentTermDtos> GetPaymentTerms();
        PaymentTermDtos GetPaymentTerm(string Code);
    }
}
