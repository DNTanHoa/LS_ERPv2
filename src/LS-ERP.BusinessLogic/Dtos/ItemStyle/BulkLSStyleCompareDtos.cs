using Common.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Dtos
{
    public class BulkLSStyleCompareDtos : IRequest<CommonCommandResultHasData<IEnumerable<LSStyleCompareDtos>>>
    {
        public List<LSStyleCompareDtos> Data { get; set; }
    }
}
