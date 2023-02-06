using Common.Model;
using LS_ERP.EntityFrameworkCore.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpdateBulkCuttingCardCommand : CommonAuditCommand,
        IRequest<CommonCommandResultHasData<CuttingCard>>
    {
        public List<string> Ids { get; set; }
        public StorageBinEntry StorageBinEntry { get; set; }        
        public string CurrentOperation { get; set; }
        public bool IsCompling { get; set; }
        public DeliveryNote DeliveryNote  { get; set; }

    }
}
