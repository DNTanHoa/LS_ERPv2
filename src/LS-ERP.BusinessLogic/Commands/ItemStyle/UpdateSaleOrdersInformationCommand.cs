using Common.Model;
using LS_ERP.BusinessLogic.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateSaleOrdersInformationCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<IEnumerable<ItemStyleInforDtos>>>
    {
        public IFormFile File { get; set; }
    }
}
