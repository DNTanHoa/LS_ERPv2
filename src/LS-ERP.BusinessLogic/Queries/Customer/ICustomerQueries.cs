using LS_ERP.BusinessLogic.Dtos.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries.Customer
{
    public interface ICustomerQueries
    {
        IEnumerable<CustomerSummaryDtos> GetCustomerSummary();
        CustomerDetailDtos GetCustomerByID(string id);
    }
}
