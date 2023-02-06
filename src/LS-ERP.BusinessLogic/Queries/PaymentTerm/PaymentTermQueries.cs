using AutoMapper;
using LS_ERP.BusinessLogic.Dtos.PaymentTerm;
using LS_ERP.EntityFrameworkCore.Shared.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public class PaymentTermQueries : IPaymentTermQueries
    {
        private readonly IMapper mapper;
        private readonly IPaymentTermRepository paymentTermRepository;

        public PaymentTermQueries(IMapper mapper,
            IPaymentTermRepository paymentTermRepository)
        {
            this.mapper = mapper;
            this.paymentTermRepository = paymentTermRepository;
        }

        public PaymentTermDtos GetPaymentTerm(string Code)
        {
            var paymentTerm = paymentTermRepository.GetPaymentTerm(Code);

            if(paymentTerm != null)
            {
                return mapper.Map<PaymentTermDtos>(paymentTerm);
            }

            return null;
        }

        public IEnumerable<PaymentTermDtos> GetPaymentTerms()
        {
            var paymentTerms = paymentTermRepository.GetPaymentTerms();
            return paymentTerms.Select(x => mapper.Map<PaymentTermDtos>(x));
        }
    }
}
