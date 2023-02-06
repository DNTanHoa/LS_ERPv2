using LS_ERP.BusinessLogic.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Queries
{
    public interface IItemStyleQueries
    {
        public IEnumerable<MaterialDtos> GetMaterials(string styles);
        public IEnumerable<CustomerItemStyleDtos> GetCustomerStyles(string customerId);
        public IEnumerable<LSStyleDtos> GetLSStyles(string customerStyle);
        public IEnumerable<LSStyleDtos> GetAllLSStyles();
        public IEnumerable<PONumberDto> GetAllPONumbers(string customerId);
    }
}
