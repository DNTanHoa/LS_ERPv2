﻿using LS_ERP.BusinessLogic.Commands.Result;
using LS_ERP.BusinessLogic.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS_ERP.BusinessLogic.Commands
{
    public class UpVersionCommand : CommonAuditCommand,
        IRequest<UpVersionResult>
    {
        public long PartRevisionID { get; set; }
        public string RevisionNumber { get; set; }
        public string Season { get; set; }
        public DateTime? EffectDate { get; set; }
    }
}